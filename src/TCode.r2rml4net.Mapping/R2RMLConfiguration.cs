using System;
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.Validation;
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

        IGraph _graphCopy;
        private ISqlVersionValidator _sqlVersionValidator = new Wc3SqlVersionValidator();
        ISqlQueryBuilder _sqlQueryBuilder = new W3CSqlQueryBuilder();

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings
        /// </summary>
        /// <param name="baseUri">base URI for mapping nodes</param>
        public R2RMLConfiguration(Uri baseUri)
            : base(baseUri, new MappingOptions())
        {
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings
        /// </summary>
        /// <param name="baseUri">base URI for mapping nodes</param>
        public R2RMLConfiguration(Uri baseUri, MappingOptions mappingOptions)
            : base(baseUri, mappingOptions)
        {
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings 
        /// and base URI set to <see cref="DefaultBaseUri"/>
        /// </summary>
        public R2RMLConfiguration()
            : this(new MappingOptions())
        {
        }

        public R2RMLConfiguration(MappingOptions mappingOptions)
            : base(DefaultBaseUri, mappingOptions)
        {

        }

        internal R2RMLConfiguration(IGraph mappingsGraph, MappingOptions mappingOptions)
            : base(mappingsGraph, mappingOptions)
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
                var triplesMapConfiguration = new TriplesMapConfiguration(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), triplesMapNode);
                triplesMaps.Add(triplesMapNode, triplesMapConfiguration);
                _triplesMaps.Add(triplesMapConfiguration);
            }

            foreach (var triplesMapPair in triplesMaps)
            {
                triplesMapPair.Value.RecursiveInitializeSubMapsFromCurrentGraph();
            }
        }

        protected override bool UsesNode
        {
            get
            {
                return false;
            }
        }

        #endregion

        void R2RMLMappingsChanged(object sender, GraphEventArgs args)
        {
            _graphCopy = null;
        }

        /// <summary>
        /// Creates a Triples Map with physical table datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromTable(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), tablename));
        }

        /// <summary>
        /// Creates a Triples Map with R2RML view datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromSqlQuery(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), sqlQuery));
        }

        private TriplesMapConfiguration SetupTriplesMap(TriplesMapConfiguration triplesMapConfiguration)
        {
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        public Uri BaseUri
        {
            get { return R2RMLMappings.BaseUri; }
        }

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

        #region Implementation of IR2RML

        /// <summary>
        /// Gets or sets the <see cref="ISqlQueryBuilder"/>
        /// </summary>
        public ISqlQueryBuilder SqlQueryBuilder
        {
            get { return _sqlQueryBuilder; }
            set { _sqlQueryBuilder = value; }
        }

        public ISqlVersionValidator SqlVersionValidator
        {
            get { return _sqlVersionValidator; }
            set { _sqlVersionValidator = value; }
        }

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