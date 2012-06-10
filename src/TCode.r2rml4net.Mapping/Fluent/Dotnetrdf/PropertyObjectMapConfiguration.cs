using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    public class PropertyObjectMapConfiguration : BaseConfiguration, IPropertyObjectMapConfiguration
    {
        private readonly IUriNode _triplesMapNode;

        public PropertyObjectMapConfiguration(IUriNode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            _triplesMapNode = triplesMapNode;
        }

        #region Implementation of IPropertyObjectMapConfiguration

        public ITermMapConfiguration AddObjectMap()
        {
            throw new System.NotImplementedException();
        }

        public ITermMapConfiguration AddPropertyMap()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}