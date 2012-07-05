using System;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    internal class GraphMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IGraphMap
    {
        internal GraphMapConfiguration(INode parentMapNode, IGraph r2RMLMappings) : base(parentMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

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

        #region Overrides of BaseConfiguration

        protected internal override void RecursiveInitializeSubMapsFromCurrentGraph()
        {
            var graphMapPropety = R2RMLMappings.CreateUriNode(R2RMLUris.RrGraphMapPropety);
            var graphTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, graphMapPropety);

            TermMapNode = graphTriples.Single().Object;
        }

        #endregion
    }
}