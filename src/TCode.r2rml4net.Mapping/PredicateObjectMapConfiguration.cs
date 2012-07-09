using System.Linq;
using System.Collections.Generic;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration, IPredicateObjectMap
    {
        private readonly INode _predicateObjectMapNode;
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<RefObjectMapConfiguration> _refObjectMaps = new List<RefObjectMapConfiguration>();
        private readonly IList<PredicateMapConfiguration> _propertyMaps = new List<PredicateMapConfiguration>();
        private readonly IList<GraphMapConfiguration> _graphMaps = new List<GraphMapConfiguration>();

        internal PredicateObjectMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _predicateObjectMapNode = R2RMLMappings.CreateBlankNode();
            R2RMLMappings.Assert(triplesMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrPredicateObjectMapPropety), _predicateObjectMapNode);
        }

        #region Implementation of IPredicateObjectMapConfiguration

        public IObjectMapConfiguration CreateObjectMap()
        {
            if (_refObjectMaps.Any())
                throw new InvalidTriplesMapException("Cannot create object map because predicate-object map already contains one or more ref object map");

            var objectMap = new ObjectMapConfiguration(_predicateObjectMapNode, R2RMLMappings);
            _objectMaps.Add(objectMap);
            return objectMap;
        }

        public ITermMapConfiguration CreatePredicateMap()
        {
            var propertyMap = new PredicateMapConfiguration(_predicateObjectMapNode, R2RMLMappings);
            _propertyMaps.Add(propertyMap);
            return propertyMap;
        }

        public IGraphMap CreateGraphMap()
        {
            var graphMap = new GraphMapConfiguration(_predicateObjectMapNode, R2RMLMappings);
            _graphMaps.Add(graphMap);
            return graphMap;
        }

        public IRefObjectMapConfiguration CreateRefObjectMap()
        {
            if (_objectMaps.Any())
                throw new InvalidTriplesMapException("Cannot create ref object map because predicate-object map already contains one or more object map");

            var refObjectMap = new RefObjectMapConfiguration(_predicateObjectMapNode, R2RMLMappings);
            _refObjectMaps.Add(refObjectMap);
            return refObjectMap;
        }

        #endregion

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Implementation of IPredicateObjectMap

        public IEnumerable<IObjectMap> ObjectMaps
        {
            get { return _objectMaps; }
        }

        public IEnumerable<IRefObjectMap> RefObjectMaps
        {
            get { return _refObjectMaps; }
        }

        #endregion
    }
}