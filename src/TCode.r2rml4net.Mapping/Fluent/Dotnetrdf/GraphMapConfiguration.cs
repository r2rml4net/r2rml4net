using System;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class GraphMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IGraphMap
    {
        internal GraphMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrGraphPropety);
        }

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrGraphMapPropety);
        }

        public override ITermMapConfiguration IsBlankNode()
        {
            throw new InvalidTriplesMapException("Only object map and subject map can be of term type rr:Literal");
        }

        #endregion

        #region Implementation of IGraphMap

        public Uri Graph
        {
            get { return ConstantValue; }
        }

        #endregion
    }
}