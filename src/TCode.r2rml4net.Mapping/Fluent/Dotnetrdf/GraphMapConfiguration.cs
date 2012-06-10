using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    public class GraphMapConfiguration : TermMapConfiguration
    {
        public GraphMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

        protected override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(RrGraphPropety);
        }

        #endregion
    }
}