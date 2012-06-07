using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class R2RMLConfiguration : IR2RMLConfiguration
    {
        public IGraph R2RMLMappings { get; internal set; }

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            if (tablename == null)
                throw new System.ArgumentNullException("tablename");
            if (string.IsNullOrWhiteSpace(tablename))
                throw new System.ArgumentOutOfRangeException("tablename");

            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings);
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            if (sqlQuery == null)
                throw new System.ArgumentNullException("sqlQuery");
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new System.ArgumentOutOfRangeException("sqlQuery");

            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings);
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }
    }
}