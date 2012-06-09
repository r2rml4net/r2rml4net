using System;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Base fluent configuration of term maps (subject maps, predicate maps, graph maps or object maps) 
    /// backed by a DotNetRDF graph (see <see cref="ITermMapConfiguration"/>)
    /// </summary>
    public abstract class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ITermTypeConfiguration
    {
        internal INode TriplesMapNode { get; private set; }
        internal INode TermMapNode { get; private set; }

        protected TermMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            TriplesMapNode = triplesMapNode;
            TermMapNode = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(TriplesMapNode, R2RMLMappings.CreateUriNode(RrSubjectMapProperty), TermMapNode);
        }

        #region Implementation of ITermMapConfiguration

        public Uri TermTypeIRI
        {
            get { throw new NotImplementedException(); }
        }

        public ITermTypeConfiguration TermType()
        {
            return this;
        }

        #endregion

        #region Implementation of ITermTypeConfiguration

        public abstract ITermMapConfiguration IsBlankNode();
        public abstract ITermMapConfiguration IsIRI();
        public abstract ITermMapConfiguration IsLiteral();

        #endregion
    }
}
