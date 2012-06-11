using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration
    {
        private readonly INode _predicateObjectMapNode;
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<PredicateMapConfiguration> _propertyMaps = new List<PredicateMapConfiguration>();

        internal PredicateObjectMapConfiguration(IUriNode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _predicateObjectMapNode = R2RMLMappings.CreateBlankNode();
            R2RMLMappings.Assert(triplesMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrPredicateObjectMapPropety), _predicateObjectMapNode);
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

        #endregion
    }
}