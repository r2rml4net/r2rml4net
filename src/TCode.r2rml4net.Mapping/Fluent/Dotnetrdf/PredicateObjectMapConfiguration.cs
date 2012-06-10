using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration
    {
        private readonly IUriNode _triplesMapNode;
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<PredicateMapConfiguration> _propertyMaps = new List<PredicateMapConfiguration>();

        internal PredicateObjectMapConfiguration(IUriNode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _triplesMapNode = triplesMapNode;
        }

        #region Implementation of IPredicateObjectMapConfiguration

        public ITermMapConfiguration CreateObjectMap()
        {
            var objectMap = new ObjectMapConfiguration(_triplesMapNode, R2RMLMappings);
            _objectMaps.Add(objectMap);
            return objectMap;
        }

        public ITermMapConfiguration CreatePredicateMap()
        {
            var propertyMap = new PredicateMapConfiguration(_triplesMapNode, R2RMLMappings);
            _propertyMaps.Add(propertyMap);
            return propertyMap;
        }

        #endregion
    }
}