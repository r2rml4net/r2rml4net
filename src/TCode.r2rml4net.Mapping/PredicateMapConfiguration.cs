using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    internal class PredicateMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IPredicateMap
    {
        internal PredicateMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IMapBase parentMap, IGraph r2RMLMappings)
            : this(parentTriplesMap, parentMap, r2RMLMappings, r2RMLMappings.CreateBlankNode())
        {
        }

        internal PredicateMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IMapBase parentMap, IGraph r2RMLMappings, INode node)
            : base(parentTriplesMap, parentMap, r2RMLMappings, node)
        {
        }

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrPredicateMapPropety);
        }

        protected internal override IUriNode CreateShortcutPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrPredicatePropety);
        }

        public override ITermMapConfiguration IsBlankNode()
        {
            throw new InvalidTriplesMapException("Only object map and subject map can be of term type rr:BlankNode");
        }

        #endregion

        #region Implementation of IPredicateMap

        public System.Uri URI
        {
            get { return ConstantValue; }
        }

        #endregion

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            // predicate maps have no submaps
        }

        #endregion
    }
}