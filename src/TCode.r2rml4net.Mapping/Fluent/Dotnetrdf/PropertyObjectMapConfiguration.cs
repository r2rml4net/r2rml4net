using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class PropertyObjectMapConfiguration : BaseConfiguration, IPropertyObjectMapConfiguration
    {
        private readonly IUriNode _triplesMapNode;
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<PropertyMapConfiguration> _propertyMaps = new List<PropertyMapConfiguration>();

        internal PropertyObjectMapConfiguration(IUriNode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _triplesMapNode = triplesMapNode;
        }

        #region Implementation of IPropertyObjectMapConfiguration

        public ITermMapConfiguration CreateObjectMap()
        {
            var objectMap = new ObjectMapConfiguration(_triplesMapNode, R2RMLMappings);
            _objectMaps.Add(objectMap);
            return objectMap;
        }

        public ITermMapConfiguration CreatePropertyMap()
        {
            var propertyMap = new PropertyMapConfiguration(_triplesMapNode, R2RMLMappings);
            _propertyMaps.Add(propertyMap);
            return propertyMap;
        }

        #endregion
    }
}