using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using System.Text.RegularExpressions;
using System.Text;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Implementation of fluent configuration interface for <a href="http://www.w3.org/TR/r2rml/#triples-map">Triples Maps</a>
    /// </summary>
    internal class TriplesMapConfiguration : BaseConfiguration, ITriplesMapFromR2RMLViewConfiguration
    {
        private readonly IR2RMLConfiguration _r2RMLConfiguration;

        private static readonly Regex TableNameRegex = new Regex(@"([\p{L}0-9 _]+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private SubjectMapConfiguration _subjectMapConfiguration;
        private readonly IList<PredicateObjectMapConfiguration> _predicateObjectMaps = new List<PredicateObjectMapConfiguration>();

        internal TriplesMapConfiguration(IR2RMLConfiguration r2RMLConfiguration, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _r2RMLConfiguration = r2RMLConfiguration;
        }

        #region Implementation of ITriplesMapConfiguration

        /// <summary>
        /// <see cref="ITriplesMap.TableName"/>
        /// </summary>
        public string TableName
        {
            get
            {
                if (Node != null)
                {
                    var sparqlQuery = new SparqlParameterizedString(
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>

SELECT ?tableName
WHERE 
{{
    @triplesMap rr:logicalTable ?lt .
    ?lt rr:tableName ?tableName
}}");

                    sparqlQuery.SetParameter("triplesMap", Node);
                    var result = (SparqlResultSet)R2RMLMappings.ExecuteQuery(sparqlQuery);

                    if (result.Count > 1)
                        throw new InvalidTriplesMapException("Triples map contains multiple table names", Uri);

                    if (result.Count == 1)
                        return result[0].Value("tableName").ToString();
                }
                return null;
            }
            internal set
            {
                if (this.TableName != null)
                    throw new InvalidTriplesMapException("Table name already set", Uri);
                if (this.SqlQuery != null)
                    throw new InvalidTriplesMapException("Cannot set both table name and SQL query", Uri);

                if (value == null)
                    throw new ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value");

                string tablename = TrimTableName(value);
                if (tablename == string.Empty)
                    throw new ArgumentOutOfRangeException("value", "The table name seems invalid");

                AssertTableNameTriples(tablename);
            }
        }

        private static string TrimTableName(string tablename)
        {
            var regexMatch = TableNameRegex.Match(tablename);

            StringBuilder stringBuilder = new StringBuilder(tablename.Length);

            if (regexMatch.Success)
            {
                stringBuilder.Append(regexMatch.Value);
                regexMatch = regexMatch.NextMatch();

                while (regexMatch.Success)
                {
                    stringBuilder.AppendFormat(".{0}", regexMatch.Value);
                    regexMatch = regexMatch.NextMatch();
                }
            }

            return stringBuilder.ToString();
        }

        private void AssertTableNameTriples(string tablename)
        {
            Node = R2RMLMappings.CreateUriNode(new Uri(string.Format("{0}TriplesMap", tablename), UriKind.Relative));

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(out tableDefinition);

            var tableName = R2RMLMappings.CreateUriNode(R2RMLUris.RrTableNameProperty);
            var tableNameLiteral = R2RMLMappings.CreateLiteralNode(tablename);

            R2RMLMappings.Assert(tableDefinition, tableName, tableNameLiteral);
        }

        private void AssertSqlQueryTriples(string sqlQuery)
        {
            Node = R2RMLMappings.CreateBlankNode();

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(out tableDefinition);

            var sqlQueryLiteral = R2RMLMappings.CreateLiteralNode(sqlQuery);
            var sqlQueryProperty = R2RMLMappings.CreateUriNode(R2RMLUris.RrSqlQueryProperty);

            R2RMLMappings.Assert(tableDefinition, sqlQueryProperty, sqlQueryLiteral);
        }

        /// <summary>
        /// <see cref="ITriplesMap.SqlQuery"/>
        /// </summary>
        public string SqlQuery
        {
            get
            {
                if (Node != null)
                {
                    var sparqlQuery = new SparqlParameterizedString(
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>

SELECT ?sqlQuery
WHERE 
{{
    @triplesMap rr:logicalTable ?lt .
    ?lt rr:sqlQuery ?sqlQuery
}}");
                    sparqlQuery.SetParameter("triplesMap", Node);
                    var result = (SparqlResultSet)R2RMLMappings.ExecuteQuery(sparqlQuery);

                    if (result.Count > 1)
                        throw new InvalidTriplesMapException("Triples map contains multiple SQL queries", Uri);

                    if (result.Count == 1)
                        return result[0].Value("sqlQuery").ToString();
                }
                return null;
            }
            internal set
            {
                if (this.SqlQuery != null)
                    throw new InvalidTriplesMapException("SQL query already set", Uri);
                if (this.TableName != null)
                    throw new InvalidTriplesMapException("Cannot set both table name and SQL query", Uri);

                if (value == null)
                    throw new ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value");

                AssertSqlQueryTriples(value);
            }
        }

        /// <summary>
        /// <see cref="ITriplesMapConfiguration.Uri"/>
        /// </summary>
        public Uri Uri
        {
            get
            {
                if (Node== null || !(Node is IUriNode))
                    return null;

                return (Node as IUriNode).Uri;
            }
        }

        private void AssertTriplesMapsTriples(out IBlankNode tableDefinition)
        {
            var tripleMapClass = R2RMLMappings.CreateUriNode(R2RMLUris.RrTriplesMapClass);
            var type = R2RMLMappings.CreateUriNode(R2RMLUris.RdfType);
            var logicalTable = R2RMLMappings.CreateUriNode(R2RMLUris.RrLogicalTableProperty);
            tableDefinition = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(Node, type, tripleMapClass);
            R2RMLMappings.Assert(Node, logicalTable, tableDefinition);
        }

        /// <summary>
        /// <see cref="ITriplesMapConfiguration.SubjectMap"/>
        /// </summary>
        ISubjectMapConfiguration ITriplesMapConfiguration.SubjectMap
        {
            get
            {
                AssertTriplesMapInitialized();

                if (_subjectMapConfiguration == null)
                    _subjectMapConfiguration= new SubjectMapConfiguration(this, R2RMLMappings);

                return _subjectMapConfiguration;
            }
        }

        /// <summary>
        /// Gets the effective sql query based on <see cref="TableName"/> or <see cref="SqlQuery"/>
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        public string EffectiveSqlQuery
        {
            get
            {
                return R2RMLConfiguration.EffectiveSqlBuilder.GetEffectiveQueryForTriplesMap(this);
            }
        }

        /// <summary>
        /// <see cref="ITriplesMapConfiguration.CreatePropertyObjectMap"/>
        /// </summary>
        public IPredicateObjectMapConfiguration CreatePropertyObjectMap()
        {
            AssertTriplesMapInitialized();

            var propertyObjectMap = new PredicateObjectMapConfiguration(this, R2RMLMappings);
            _predicateObjectMaps.Add(propertyObjectMap);
            return propertyObjectMap;
        }

        public IR2RMLConfiguration R2RMLConfiguration
        {
            get { return _r2RMLConfiguration; }
        }

        #endregion

        #region Implementation of ITriplesMapFromR2RMLViewConfiguration

        /// <summary>
        /// Asserts this Triples Map's SQL query as a query of type defined by <paramref name="uri"/> parmeter
        /// </summary>
        /// <param name="uri">Usually on of the URIs listed on http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs </param>
        public ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(Uri uri)
        {
            if (TableName != null)
                throw new InvalidTriplesMapException("Cannot set SQL version to a table-based logical table", Uri);

            R2RMLMappings.Assert(LogicalTableNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrSqlVersionProperty), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        /// <summary>
        /// <see cref="SetSqlVersion(System.Uri)"/>
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(string uri)
        {
            return this.SetSqlVersion(new Uri(uri));
        }

        /// <summary>
        /// <see cref="ITriplesMapFromR2RMLViewConfiguration.SqlVersions"/>
        /// </summary>
        public Uri[] SqlVersions
        {
            get
            {
                IBlankNode logicalTableNode = LogicalTableNode;

                if (logicalTableNode == null)
                    return new Uri[0];

                var triples = R2RMLMappings.GetTriplesWithSubjectPredicate(logicalTableNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrSqlVersionProperty));
                return triples.Select(triple => ((IUriNode)triple.Object).Uri).ToArray();
            }
        }

        #endregion

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            CreateSubMaps(R2RMLUris.RrPredicateObjectMapPropety, graph => new PredicateObjectMapConfiguration(this, graph), _predicateObjectMaps);

            var subjectMaps = new List<SubjectMapConfiguration>();
            CreateSubMaps(R2RMLUris.RrSubjectMapProperty, graph => new SubjectMapConfiguration(this, graph), subjectMaps);

            if(subjectMaps.Count > 1)
                throw new InvalidTriplesMapException("Triples map can only have one subject map");
            _subjectMapConfiguration = subjectMaps.SingleOrDefault();
        }

        protected internal override ITriplesMapConfiguration TriplesMap
        {
            get { return this; }
        }

        #endregion

        #region Implementation of ITriplesMap

        public IEnumerable<IPredicateObjectMap> PredicateObjectMaps
        {
            get { return _predicateObjectMaps; }
        }

        public ISubjectMap SubjectMap
        {
            get
            {
                AssertTriplesMapInitialized();

                if (_subjectMapConfiguration == null)
                    _subjectMapConfiguration = new SubjectMapConfiguration(this, R2RMLMappings);

                return _subjectMapConfiguration;
            }
        }

        #endregion

        IBlankNode LogicalTableNode
        {
            get
            {
                var logicalTables = R2RMLMappings.GetTriplesWithSubjectPredicate(
                    Node,
                    R2RMLMappings.CreateUriNode(R2RMLUris.RrLogicalTableProperty)
                    ).ToArray();

                if (logicalTables.Count() > 1)
                    throw new InvalidTriplesMapException("Triples Map contains multiple logical tables!", Uri);

                return logicalTables.First().Object as IBlankNode;
            }
        }

        void AssertTriplesMapInitialized()
        {
            if (Uri == null)
                throw new InvalidOperationException("Triples map hasn't been initialized yet. Please set the TableName or SqlQuery property");
        }
    }
}