using System;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

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

        protected internal override IUriNode CreateShortcutPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrGraphPropety);
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

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            var graphMapPropety = R2RMLMappings.CreateUriNode(R2RMLUris.RrGraphMapPropety);
            var graphTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, graphMapPropety);

            Triple graphTriple = graphTriples.SingleOrDefault();

            if (graphTriple != null)
                TermMapNode = graphTriple.Object;
            else
                throw new InvalidOperationException(string.Format("Cannot initialize. Subject Map {0} has no graph map", ParentMapNode));
        }

        #endregion
    }
}