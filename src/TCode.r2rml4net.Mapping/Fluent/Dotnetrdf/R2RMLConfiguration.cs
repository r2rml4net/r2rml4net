using System;
using System.Linq;
using System.Collections.Generic;
using VDS.RDF;
using System.Text.RegularExpressions;
using System.Text;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class R2RMLConfiguration : BaseConfiguration, IR2RMLConfiguration
    {
        internal static Uri DefaultBaseUri
        {
            get
            {
                return new Uri("http://r2rml.net/mappings#");
            }
        }

        protected Uri MappingBaseUri
        {
            get { return new Uri("http://example.com/"); }
        }

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        public R2RMLConfiguration(Uri baseUri)
            : base(baseUri)
        {
        }

        public R2RMLConfiguration()
            : base(DefaultBaseUri)
        {
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