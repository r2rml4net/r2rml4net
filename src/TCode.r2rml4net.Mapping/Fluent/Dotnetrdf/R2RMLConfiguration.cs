using System;
using System.Linq;
using System.Collections.Generic;
using VDS.RDF;
using System.Text.RegularExpressions;
using System.Text;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class R2RMLConfiguration : IR2RMLConfiguration
    {
        private readonly Uri _baseUri;

        internal static Uri DefaultBaseUri
        {
            get
            {
                return new Uri("http://r2rml.net/mappings#");
            }
        }

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

        public R2RMLConfiguration(Uri baseUri)
            : this()
        {
            _baseUri = baseUri;
        }

        public R2RMLConfiguration()
        {
            _baseUri = DefaultBaseUri;
        }

        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings);
            triplesMapConfiguration.TableName = tablename;
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings);
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }
    }
}