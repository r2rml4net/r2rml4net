using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class GraphMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion
    {
        internal GraphMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(UrisHelper.RrGraphPropety);
        }

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(UrisHelper.RrGraphMapPropety);
        }

        public override ITermMapConfiguration IsBlankNode()
        {
            throw new InvalidTriplesMapException("Only object map and subject map can be of term type rr:Literal");
        }

        #endregion
    }
}