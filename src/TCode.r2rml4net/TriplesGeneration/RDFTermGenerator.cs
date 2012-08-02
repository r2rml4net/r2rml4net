using System;
using System.Data;
using System.Text.RegularExpressions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB.ADO.NET;
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
        private ISQLValuesMappingStrategy _sqlValuesMappingStrategy = new DefaultSQLValuesMappingStrategy();
        private IRDFTermGenerationLog _log = NullLog.Instance;

        public ISQLValuesMappingStrategy SqlValuesMappingStrategy
        {
            get { return _sqlValuesMappingStrategy; }
            set { _sqlValuesMappingStrategy = value; }
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

            var logicalRowWrapped = new UndelimitedColumnsDataRecordWrapper(logicalRow);

            if(!(termMap.IsColumnValued || termMap.IsConstantValued || termMap.IsTemplateValued))
                throw new InvalidTermException(termMap, "Term map must be either constant, column or template valued");

            if (termMap.IsConstantValued)
            {
                node = CreateNodeFromConstant(termMap);
            }
            else if (termMap.IsColumnValued)
            {
                node = CreateNodeFromColumn(termMap, logicalRowWrapped);
            }
            else if (termMap.IsTemplateValued)
            {
                node = CreateNodeFromTemplate(termMap, logicalRowWrapped);
            }

            if (node == null)
                Log.LogNullTermGenerated(termMap);
            else Log.LogTermGenerated(node);

            return (TNodeType)node;
        }

        #endregion

        protected INode CreateNodeFromTemplate(ITermMap termMap, IDataRecord logicalRow)
        {
            if (string.IsNullOrWhiteSpace(termMap.Template))
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

        protected INode CreateNodeFromConstant(ITermMap termMap)
        {
            var uriValuedTermMap = termMap as IUriValuedTermMap;
            if (uriValuedTermMap != null)
            {
                if (uriValuedTermMap.URI == null)
                    throw new InvalidTermException(termMap, "IRI-valued term map must have IRI set");

                return NodeFactory.CreateUriNode(uriValuedTermMap.URI);
            }

            var objectMap = termMap as IObjectMap;
            if (objectMap != null)
            {
                if (objectMap.URI != null && objectMap.Literal != null)
                    throw new InvalidTermException(termMap, "Object map's value cannot be both IRI and literal.");

                if (objectMap.URI != null)
                    return NodeFactory.CreateUriNode(objectMap.URI);
                if (objectMap.Literal != null)
                    return NodeFactory.CreateLiteralNode(objectMap.Literal);

                throw new InvalidTermException(termMap, "Neither IRI nor literal was set");
            }

            return null;
        }

        protected INode CreateNodeFromColumn(ITermMap termMap, IDataRecord logicalRow)
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
            {
                Log.LogNullValueForColumn(termMap.ColumnName);
                return null;
            }

            Uri implicitDatatype;
            string value = SqlValuesMappingStrategy.GetLexicalForm(columnIndex, logicalRow, out implicitDatatype);

            if (termMap.TermType.IsLiteral)
            {
                return GenerateTermForLiteral(termMap, value, implicitDatatype);
            }

            return GenerateTermForValue(termMap, value);
        }

        internal INode GenerateTermForValue(ITermMap termMap, string value)
        {
            if (value == null)
                return null;

            if (termMap.TermType.IsURI)
            {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return NodeFactory.CreateUriNode(new Uri(value));

                value = termMap.BaseURI + value;
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    return NodeFactory.CreateUriNode(new Uri(value));

                throw new InvalidTermException(termMap, string.Format("Value {0} is invalid", value));
            }
            if (termMap.TermType.IsBlankNode)
            {
                if (!ValidBlankNodeRegex.IsMatch(value))
                    Log.LogInvalidBlankNode(termMap, value);

                return NodeFactory.CreateBlankNode(value);
            }
            if (termMap.TermType.IsLiteral)
            {
                var literalTermMap = termMap as ILiteralTermMap;

                return GenerateTermForLiteral(literalTermMap, value);
            }

            throw new InvalidTermException(termMap, "Term map must be either IRI-, literal- or blank node-valued");
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
            {
                Log.LogNullValueForColumn(columnName);
                throw new ArgumentNullException();
            }

            Uri datatype;
            return SqlValuesMappingStrategy.GetLexicalForm(columnIndex, logicalRow, out datatype);
        }

        private ILiteralNode GenerateTermForLiteral(ITermMap termMap, string value, Uri datatypeUriOverride = null)
        {
            if (!(termMap is ILiteralTermMap))
                throw new InvalidTermException(termMap, "Term map cannot be of term type literal");

            var literalTermMap = termMap as ILiteralTermMap;
            Uri datatypeUri = datatypeUriOverride ?? literalTermMap.DataTypeURI;
            string languageTag = literalTermMap.LanguageTag;

            if (languageTag != null && datatypeUri != null)
                throw new InvalidTermException(literalTermMap, "Literal term map cannot have both language tag and datatype set");

            if (languageTag != null)
                return NodeFactory.CreateLiteralNode(value, languageTag);
            if (datatypeUri != null)
                return NodeFactory.CreateLiteralNode(value, datatypeUri);

            return NodeFactory.CreateLiteralNode(value);
        }
    }
}