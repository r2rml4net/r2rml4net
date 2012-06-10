using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class PropertyMapConfiguration : TermMapConfiguration
    {
        internal PropertyMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(RrPredicatePropety);
        }

        #endregion
    }
}