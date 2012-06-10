using System;
using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Entrypoint to fluent configuration of R2RML, backed by DotNetRDF
    /// </summary>
    public class R2RMLConfiguration : BaseConfiguration, IR2RMLConfiguration
    {
        internal static Uri DefaultBaseUri
        {
            get
            {
                return new Uri("http://r2rml.net/mappings#");
            }
        }

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings
        /// </summary>
        /// <param name="baseUri">base URI for mapping nodes</param>
        public R2RMLConfiguration(Uri baseUri)
            : base(baseUri)
        {
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        void R2RMLMappingsChanged(object sender, GraphEventArgs args)
        {
            _graphCopy = null;
        }

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings 
        /// and base URI set to <see cref="DefaultBaseUri"/>
        /// </summary>
        public R2RMLConfiguration()
            : this(DefaultBaseUri)
        {
        }

        /// <summary>
        /// Creates a Triples Map with physical table datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings) { TableName = tablename };
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        /// <summary>
        /// Creates a Triples Map with R2RML view datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            var triplesMapConfiguration = new TriplesMapConfiguration(R2RMLMappings) { SqlQuery = sqlQuery };
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        IGraph _graphCopy;
        /// <summary>
        /// Returns copy of the mapping graph
        /// </summary>
        public IGraph GraphReadOnly
        {
            get
            {
                if (_graphCopy == null)
                {
                    _graphCopy = new Graph(R2RMLMappings.Triples);
                    _graphCopy.NamespaceMap.Import(R2RMLMappings.NamespaceMap);
                }

                return _graphCopy;
            }
        }
    }
}