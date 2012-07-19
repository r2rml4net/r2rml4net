using System;
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
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
                return new Uri("http://r2rml.net/mappings/");
            }
        }

        IEffectiveSqlBuilder _effectiveSqlBuilder = new W3CEffectiveSqlBuilder();
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

        internal R2RMLConfiguration(IGraph mappingsGraph)
            : base(mappingsGraph)
        {

        }

        #region Overrides of BaseConfiguration

        /// <summary>
        /// Creates triples maps configuration objects for the current mapping file
        /// </summary>
        /// <remarks>Used in loading configuration from an exinsting graph</remarks>
        protected override void InitializeSubMapsFromCurrentGraph()
        {
            if (R2RMLMappings == null)
                return;

            var rdfType = R2RMLMappings.CreateUriNode(R2RMLUris.RdfType);
            var triplesMapClass = R2RMLMappings.CreateUriNode(R2RMLUris.RrTriplesMapClass);
            var triplesMapsTriples = R2RMLMappings.GetTriplesWithPredicateObject(rdfType, triplesMapClass).ToArray();
            IDictionary<INode, TriplesMapConfiguration> triplesMaps = new Dictionary<INode, TriplesMapConfiguration>();

            foreach (var triplesMapNode in triplesMapsTriples.Select(triple => triple.Subject))
            {
                var triplesMapConfiguration = new TriplesMapConfiguration(this, R2RMLMappings);
                triplesMaps.Add(triplesMapNode, triplesMapConfiguration);
                _triplesMaps.Add(triplesMapConfiguration);
            }

            foreach (var triplesMapPair in triplesMaps)
            {
                triplesMapPair.Value.RecursiveInitializeSubMapsFromCurrentGraph(triplesMapPair.Key);
            }
        }

        public override INode Node
        {
            get { return null; }
        }

        #endregion

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
            var triplesMapConfiguration = new TriplesMapConfiguration(this, R2RMLMappings) { TableName = tablename };
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        /// <summary>
        /// Creates a Triples Map with R2RML view datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            var triplesMapConfiguration = new TriplesMapConfiguration(this, R2RMLMappings) { SqlQuery = sqlQuery };
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
                    _graphCopy.BaseUri = R2RMLMappings.BaseUri;
                }

                return _graphCopy;
            }
        }

        public IEffectiveSqlBuilder EffectiveSqlBuilder
        {
            get { return _effectiveSqlBuilder; }
            set { _effectiveSqlBuilder = value; }
        }

        #region Implementation of IR2RML

        /// <summary>
        /// <see cref="IR2RML.TriplesMaps"/>
        /// </summary>
        public IEnumerable<ITriplesMap> TriplesMaps
        {
            get { return _triplesMaps; }
        }

        #endregion
    }
}