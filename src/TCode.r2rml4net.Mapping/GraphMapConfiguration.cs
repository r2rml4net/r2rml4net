using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    internal class GraphMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IGraphMap
    {
        internal GraphMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraphMapParent parentMap, IGraph r2RMLMappings)
            : this(parentTriplesMap, parentMap, r2RMLMappings, r2RMLMappings.CreateBlankNode())
        {
        }

        internal GraphMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraphMapParent parentMap, IGraph r2RMLMappings, INode node) 
            : base(parentTriplesMap, parentMap, r2RMLMappings, node)
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

        public Uri URI
        {
            get { return ConstantValue; }
        }

        #endregion

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            
        }

        #endregion
    }
}