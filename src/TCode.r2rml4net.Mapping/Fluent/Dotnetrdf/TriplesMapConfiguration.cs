using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    public class TriplesMapConfiguration : ITriplesMapFromR2RMLViewConfiguration
    {
        internal IGraph R2RMLMappings { get; private set; }

        public TriplesMapConfiguration(IGraph r2RMLMappings)
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

        #endregion

        #region Implementation of ITriplesMapFromR2RMLViewConfiguration

        public ITriplesMapConfiguration SetSqlVersion(Uri uri)
        {
            return this;
        }

        public ITriplesMapConfiguration SetSqlVersion(string uri)
        {
            return this;
        }

        #endregion
    }
}