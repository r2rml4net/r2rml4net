using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class ObjectMapConfiguration : TermMapConfiguration, IObjectMapConfiguration
    {
        internal ObjectMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Implementation of IObjectMapConfiguration

        public IObjectMapConfiguration IsConstantValued(string literal)
        {
            if (R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, CreateConstantPropertyNode()).Any())
                throw new InvalidTriplesMapException("Term map can have at most one constant value");

            R2RMLMappings.Assert(TermMapNode, CreateConstantPropertyNode(), R2RMLMappings.CreateLiteralNode(literal));

            return this;
        }

        #endregion

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(RrObjectProperty);
        }

        #endregion
    }
}