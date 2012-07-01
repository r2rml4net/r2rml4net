using System.Collections.Generic;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration, IPredicateObjectMap
    {
        private readonly INode _predicateObjectMapNode;
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
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

        #endregion

        #region Overrides of BaseConfiguration

        protected internal override void RecursiveInitializeSubMapsFromCurrentGraph()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}