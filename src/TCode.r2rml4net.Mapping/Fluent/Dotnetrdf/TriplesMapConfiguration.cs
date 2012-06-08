using System;
using VDS.RDF;
using System.Text.RegularExpressions;
using System.Text;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Implementation of fluent configuration interface for <a href="http://www.w3.org/TR/r2rml/#triples-map">Triples Maps</a>
    /// </summary>
    class TriplesMapConfiguration : BaseConfiguration, ITriplesMapConfiguration, ITriplesMapFromR2RMLViewConfiguration
    {
        private static Regex TableNameRegex = new Regex("([a-zA-Z0-9]+)");
        private string _triplesMapUri;

        /// <summary>
        /// Creates an instance of <see cref="TriplesMapConfiguration"/>
        /// </summary>
        /// <param name="r2RMLMappings">existing mappings passed from <see cref="R2RMLConfiguration"/></param>
        internal TriplesMapConfiguration(IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
        }

        #region Implementation of ITriplesMapConfiguration

        public string TableName
        {
            get
            {
                if (_triplesMapUri != null)
                {
                    var result = (SparqlResultSet)R2RMLMappings.ExecuteQuery(string.Format(@"
                                    PREFIX rr: <http://www.w3.org/ns/r2rml#>

                                    SELECT ?tableName
                                    WHERE 
                                    {{
                                      <{0}> rr:logicalTable ?lt .
                                      ?lt rr:tableName ?tableName
                                    }}", _triplesMapUri));

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
                    throw new System.ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new System.ArgumentOutOfRangeException("value");

                string tablename = TrimTableName(value);
                if (tablename == string.Empty)
                    throw new System.ArgumentOutOfRangeException("tablename", "The table name seems invalid");

                AssertTableNameTriples(tablename);
            }
        }

        private string TrimTableName(string tablename)
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
            // TODO: refactor with new version of dotNetRDF
            string triplesMapUri = string.Format("{0}{1}TriplesMap", R2RMLMappings.BaseUri, tablename);

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(triplesMapUri, out tableDefinition);

            var tableName = R2RMLMappings.CreateUriNode("rr:tableName");
            var tableNameLiteral = R2RMLMappings.CreateLiteralNode(tablename);

            R2RMLMappings.Assert(tableDefinition, tableName, tableNameLiteral);
        }

        private void AssertSqlQueryTriples(string sqlQuery)
        {
            // TODO: refactor for something else than GUID
            string triplesMapUri = string.Format("{0}{1}TriplesMap", R2RMLMappings.BaseUri, Guid.NewGuid());

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(triplesMapUri, out tableDefinition);

            var sqlQueryLiteral = R2RMLMappings.CreateLiteralNode(sqlQuery);
            var sqlQueryProperty = R2RMLMappings.CreateUriNode("rr:sqlQuery");

            R2RMLMappings.Assert(tableDefinition, sqlQueryProperty, sqlQueryLiteral);
        }

        public string SqlQuery
        {
            get
            {
                if (_triplesMapUri != null)
                {
                    var result = (SparqlResultSet)R2RMLMappings.ExecuteQuery(string.Format(@"
                                    PREFIX rr: <http://www.w3.org/ns/r2rml#>

                                    SELECT ?sqlQuery
                                    WHERE 
                                    {{
                                      <{0}> rr:logicalTable ?lt .
                                      ?lt rr:sqlQuery ?sqlQuery
                                    }}", _triplesMapUri));

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
                    throw new System.ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new System.ArgumentOutOfRangeException("value");

                AssertSqlQueryTriples(value);
            }
        }

        public Uri Uri
        {
            get
            {
                return new Uri(_triplesMapUri);
            }
        }

        private void AssertTriplesMapsTriples(string triplesMapUri, out IBlankNode tableDefinition)
        {
            _triplesMapUri = triplesMapUri;

            var tripleMap = R2RMLMappings.CreateUriNode(Uri);
            var tripleMapClass = R2RMLMappings.CreateUriNode("rr:TriplesMap");
            var type = R2RMLMappings.CreateUriNode("rdf:type");
            var logicalTable = R2RMLMappings.CreateUriNode("rr:logicalTable");
            tableDefinition = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(tripleMap, type, tripleMapClass);
            R2RMLMappings.Assert(tripleMap, logicalTable, tableDefinition);
        }

        public ISubjectMapConfiguration SubjectMap()
        {
            return new TermMapConfiguration(R2RMLMappings);
        }

        #endregion

        #region Implementation of ITriplesMapFromR2RMLViewConfiguration

        /// <summary>
        /// Asserts this Triples Map's SQL query as a query of type defined by <paramref name="uri"/> parmeter
        /// </summary>
        /// <param name="uri">Usually on of the URIs listed on http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs </param>
        public ITriplesMapConfiguration SetSqlVersion(Uri uri)
        {
            return this;
        }

        /// <summary>
        /// <see cref="SetSqlVersion(Uri)"/>
        /// </summary>
        public ITriplesMapConfiguration SetSqlVersion(string uri)
        {
            return this.SetSqlVersion(new Uri(uri));
        }

        #endregion
    }
}