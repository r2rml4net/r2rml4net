using System;
using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class R2RMLConfiguration : IR2RMLConfiguration
    {
        private readonly Uri _baseUri;

        internal static Uri DefaultBaseUri{get
        {
            return new Uri("http://r2rml.net/mappings#");
        }}

        private IGraph _R2RMLMappings;
        internal IGraph R2RMLMappings
        {
            get
            {
                if (_R2RMLMappings == null)
                {
                    _R2RMLMappings = new Graph { BaseUri = _baseUri };

                    _R2RMLMappings.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
                }

                return _R2RMLMappings;
            }
        }

        protected Uri MappingBaseUri
        {
            get { return new Uri("http://example.com/"); }
        }

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        public R2RMLConfiguration(Uri baseUri):this()
        {
            _baseUri = baseUri;
        }

        public R2RMLConfiguration()
        {
            _baseUri = DefaultBaseUri;
        }

        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            if (tablename == null)
                throw new System.ArgumentNullException("tablename");
            if (string.IsNullOrWhiteSpace(tablename))
                throw new System.ArgumentOutOfRangeException("tablename");

            AssertTriples(tablename);

            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings);
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        private void AssertTriples(string tablename)
        {
            var type = R2RMLMappings.CreateUriNode("rdf:type");
            var tripleMap = R2RMLMappings.CreateUriNode("rr:TriplesMap");
            var logicalTable = R2RMLMappings.CreateUriNode("rr:logicalTable");
            var tableName = R2RMLMappings.CreateUriNode("rr:tableName");
            var tableNameLiteral = R2RMLMappings.CreateLiteralNode(tablename);
            var tableDefinition = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(tripleMap, type, tripleMap);
            R2RMLMappings.Assert(tripleMap, logicalTable, tableDefinition);
            R2RMLMappings.Assert(tableDefinition, tableName, tableNameLiteral);
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