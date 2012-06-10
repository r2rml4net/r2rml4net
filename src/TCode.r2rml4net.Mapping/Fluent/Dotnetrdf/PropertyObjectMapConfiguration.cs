using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    public class PropertyObjectMapConfiguration : TermMapConfiguration, IPropertyObjectMapConfiguration
    {
        public PropertyObjectMapConfiguration(IUriNode triplesMapNode, IGraph r2RMLMappings)
            :base(triplesMapNode, r2RMLMappings)
        {
            
        }
    }
}