using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    internal class PredicateMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IPredicateMap
    {
        internal PredicateMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IMapBase parentMap, IGraph r2RMLMappings)
            : base(parentTriplesMap, parentMap, r2RMLMappings)
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

        public System.Uri Predicate
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