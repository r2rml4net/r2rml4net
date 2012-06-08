using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Implementation of fluent configuration interface for <a href="http://www.w3.org/TR/r2rml/#triples-map">Triples Maps</a>
    /// </summary>
    class TriplesMapConfiguration : ITriplesMapConfiguration, ITriplesMapFromR2RMLViewConfiguration
    {
        internal IGraph R2RMLMappings { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="TriplesMapConfiguration"/>
        /// </summary>
        /// <param name="r2RMLMappings">existing mappings passed from <see cref="R2RMLConfiguration"/></param>
        internal TriplesMapConfiguration(IGraph r2RMLMappings)
        {
            R2RMLMappings = r2RMLMappings;
        }

        #region Implementation of ITriplesMapConfiguration

        string _tableName;
        public string TableName
        {
            get 
            {
                return _tableName;
            }
            internal set 
            {
                _tableName = value;
            }
        }

        string _sqlQuery;
        public string SqlQuery
        {
            get
            {
                return _sqlQuery;
            }
            internal set
            {
                _sqlQuery = value;
            }
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