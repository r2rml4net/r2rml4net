using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class PredicateMapConfiguration : TermMapConfiguration, INonLiteralTermMapConfigutarion, IPredicateMap
    {
        internal PredicateMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrPredicateMapPropety);
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
    }
}