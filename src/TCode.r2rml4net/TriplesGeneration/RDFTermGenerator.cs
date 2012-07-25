using System;
using System.Data;
using System.Text.RegularExpressions;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Default implementation of the <a href="http://www.w3.org/TR/r2rml/#dfn-generated-rdf-term">RDF term generation process</a>
    /// </summary>
    class RDFTermGenerator : IRDFTermGenerator
    {
        static readonly Regex TemplateReplaceRegex = new Regex(@"(?<N>\{)([ \\\"a-zA-Z0-9]+)(?<-N>\})(?(N)(?!))");
        readonly INodeFactory _nodeFactory = new NodeFactory();
        private INaturalLexicalFormProvider _lexicalFormProvider = new W3CLexicalFormProvider();

        public INaturalLexicalFormProvider LexicalFormProvider
        {
            get { return _lexicalFormProvider; }
            set { _lexicalFormProvider = value; }
        }

        #region Implementation of IRDFTermGenerator

        public TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow)
            where TNodeType : class, INode
        {
            if (termMap.IsConstantValued)
            {
                return (TNodeType) CreateConstantValue(termMap);
            }
            if (termMap.IsColumnValued)
            {
                return (TNodeType) CreateNodeFromColumn(termMap, logicalRow);
            }
            if (termMap.IsTemplateValued)
            {
                return (TNodeType)CreateNodeFromTemplate(termMap, logicalRow);
            }

            return null;
        }

        #endregion

        private INode CreateNodeFromTemplate(ITermMap termMap, IDataRecord logicalRow)
        {
            string value = ReplaceColumnReferences(termMap.Template, logicalRow);
            return GenerateTermForValue(termMap, value);
        }

        private string ReplaceColumnReferences(string template, IDataRecord logicalRow)
        {
            return TemplateReplaceRegex.Replace(template, match => ReplaceColumnReference(match, logicalRow));
        }

        private string ReplaceColumnReference(Match match, IDataRecord logicalRow)
        {
            int columnIndex = logicalRow.GetOrdinal(match.Captures[0].Value);
            return LexicalFormProvider.GetNaturalLexicalForm(columnIndex,
                                                             logicalRow);
        }

        private INode CreateNodeFromColumn(ITermMap termMap, IDataRecord logicalRow)
        {
            int columnIndex = logicalRow.GetOrdinal(termMap.ColumnName);
            if (logicalRow.IsDBNull(columnIndex))
                return null;

            string value = LexicalFormProvider.GetNaturalLexicalForm(columnIndex, logicalRow);

            return GenerateTermForValue(termMap, value);
        }

        private INode GenerateTermForValue(ITermMap termMap, string value)
        {
            if (termMap.TermType.IsURI)
            {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return _nodeFactory.CreateUriNode(new Uri(value));

                value = termMap.BaseURI + value;
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return _nodeFactory.CreateUriNode(new Uri(value));

                throw new InvalidTermException(termMap, value);
            }
            if (termMap.TermType.IsBlankNode)
            {
                return _nodeFactory.CreateBlankNode(value);
            }
            if (termMap.TermType.IsLiteral)
            {
                ILiteralTermMap literalTermMap = (ILiteralTermMap)termMap;

                if (literalTermMap.LanguageTag != null)
                    return _nodeFactory.CreateLiteralNode(value, literalTermMap.LanguageTag);
                if (literalTermMap.DataTypeURI != null)
                    return _nodeFactory.CreateLiteralNode(value, literalTermMap.DataTypeURI);

                return _nodeFactory.CreateLiteralNode(value);
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

                return _nodeFactory.CreateUriNode(uriValuedTermMap.URI);
            }

            var objectMap = termMap as IObjectMap;
            if (objectMap != null)
            {
                if (objectMap.URI != null && objectMap.Literal != null)
                    throw new InvalidTermException(string.Format("Cannot create RDF term for constant-valued term map {0}. Object map's value must be either IRI or literal.", objectMap.Node));
                
                if (objectMap.URI != null)
                    return _nodeFactory.CreateUriNode(objectMap.URI);
                if (objectMap.Literal != null)
                    return _nodeFactory.CreateLiteralNode(objectMap.Literal);

                throw new InvalidTermException(string.Format("Cannot create RDF term for constant-valued term map {0}. Neither IRI nor literal was set.", objectMap.Node));
            }

            return null;
        }
    }
}