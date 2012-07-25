using System;
using System.Data;
using System.Text.RegularExpressions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Default implementation of the <a href="http://www.w3.org/TR/r2rml/#dfn-generated-rdf-term">RDF term generation process</a>
    /// </summary>
    class RDFTermGenerator : IRDFTermGenerator
    {
        static readonly Regex ValidBlankNodeRegex = new Regex(@"^[a-zA-Z][a-zA-Z_0-9-]*$");
        static readonly Regex TemplateReplaceRegex = new Regex(@"(?<N>\{)([ \\\""a-zA-Z0-9]+)(?<-N>\})(?(N)(?!))");
        private INodeFactory _nodeFactory = new NodeFactory();
        private ILexicalFormProvider _lexicalFormProvider = new XSDLexicalFormProvider();
        private IRDFTermGenerationLog _log = NullLog.Instance;

        public ILexicalFormProvider LexicalFormProvider
        {
            get { return _lexicalFormProvider; }
            set { _lexicalFormProvider = value; }
        }

        public IRDFTermGenerationLog Log
        {
            get { return _log; }
            set { _log = value; }
        }

        public INodeFactory NodeFactory
        {
            get { return _nodeFactory; }
            set { _nodeFactory = value; }
        }

        #region Implementation of IRDFTermGenerator

        public TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow)
            where TNodeType : class, INode
        {
            INode node = null;

            if (termMap.IsConstantValued)
            {
                node = CreateConstantValue(termMap);
            }
            else if (termMap.IsColumnValued)
            {
                node = CreateNodeFromColumn(termMap, logicalRow);
            }
            else if (termMap.IsTemplateValued)
            {
                node = CreateNodeFromTemplate(termMap, logicalRow);
            }
            // an exception of blank node subject maps (direct mapping)
            else if(termMap is ISubjectMap && termMap.TermType.IsBlankNode)
            {
                node = NodeFactory.CreateBlankNode();
            }

            if (node == null)
                Log.LogNullTermGenerated(termMap);
            else Log.LogTermGenerated(node);

            return (TNodeType)node;
        }

        #endregion

        private INode CreateNodeFromTemplate(ITermMap termMap, IDataRecord logicalRow)
        {
            if(string.IsNullOrWhiteSpace(termMap.Template))
                throw new InvalidTemplateException(termMap);

            string value;
            try
            {
                value = ReplaceColumnReferences(termMap.Template, logicalRow);
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(value) && termMap.TermType.IsURI)
                value = Uri.EscapeUriString(value);

            return GenerateTermForValue(termMap, value);
        }

        private string ReplaceColumnReferences(string template, IDataRecord logicalRow)
        {
            try
            {
                return TemplateReplaceRegex.Replace(template, match => ReplaceColumnReference(match, logicalRow));
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        private string ReplaceColumnReference(Match match, IDataRecord logicalRow)
        {
            var columnName = match.Captures[0].Value.TrimStart('{').TrimEnd('}');
            int columnIndex = logicalRow.GetOrdinal(columnName);

            if (logicalRow.IsDBNull(columnIndex))
                throw new ArgumentNullException();

            return LexicalFormProvider.GetLexicalForm(columnIndex,
                                                             logicalRow);
        }

        private INode CreateNodeFromColumn(ITermMap termMap, IDataRecord logicalRow)
        {
            int columnIndex;
            try
            {
                columnIndex = logicalRow.GetOrdinal(termMap.ColumnName);
            }
            catch (IndexOutOfRangeException)
            {
                Log.LogColumnNotFound(termMap, termMap.ColumnName);
                return null;
            }
            if (logicalRow.IsDBNull(columnIndex))
                return null;

            string value = LexicalFormProvider.GetLexicalForm(columnIndex, logicalRow);

            return GenerateTermForValue(termMap, value);
        }

        internal INode GenerateTermForValue(ITermMap termMap, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (termMap.TermType.IsURI)
            {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return NodeFactory.CreateUriNode(new Uri(value));

                value = termMap.BaseURI + value;
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return NodeFactory.CreateUriNode(new Uri(value));

                throw new InvalidTermException(termMap, value);
            }
            if (termMap.TermType.IsBlankNode)
            {
                if (!ValidBlankNodeRegex.IsMatch(value))
                    Log.LogInvalidBlankNode(termMap, value);

                return NodeFactory.CreateBlankNode(value);
            }
            if (termMap.TermType.IsLiteral)
            {
                ILiteralTermMap literalTermMap = (ILiteralTermMap)termMap;

                if (literalTermMap.LanguageTag != null && literalTermMap.DataTypeURI != null)
                    throw new InvalidTermException(termMap);

                if (literalTermMap.LanguageTag != null)
                    return NodeFactory.CreateLiteralNode(value, literalTermMap.LanguageTag);
                if (literalTermMap.DataTypeURI != null)
                    return NodeFactory.CreateLiteralNode(value, literalTermMap.DataTypeURI);

                return NodeFactory.CreateLiteralNode(value);
            }

            throw new InvalidTermException(termMap);
        }

        private INode CreateConstantValue(ITermMap termMap)
        {
            var uriValuedTermMap = termMap as IUriValuedTermMap;
            if (uriValuedTermMap != null)
            {
                if (uriValuedTermMap.URI == null)
                    throw new InvalidTermException(string.Format("Cannot create RDF term for IRI-valued term map {0}. IRI was set.*", termMap.Node));

                return NodeFactory.CreateUriNode(uriValuedTermMap.URI);
            }

            var objectMap = termMap as IObjectMap;
            if (objectMap != null)
            {
                if (objectMap.URI != null && objectMap.Literal != null)
                    throw new InvalidTermException(string.Format("Cannot create RDF term for constant-valued term map {0}. Object map's value must be either IRI or literal.", objectMap.Node));

                if (objectMap.URI != null)
                    return NodeFactory.CreateUriNode(objectMap.URI);
                if (objectMap.Literal != null)
                    return NodeFactory.CreateLiteralNode(objectMap.Literal);

                throw new InvalidTermException(string.Format("Cannot create RDF term for constant-valued term map {0}. Neither IRI nor literal was set.", objectMap.Node));
            }

            return null;
        }
    }
}