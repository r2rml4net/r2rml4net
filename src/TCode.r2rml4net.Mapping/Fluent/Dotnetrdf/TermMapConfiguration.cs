using System;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ISubjectMapConfiguration
    {
        internal INode TriplesMapNode { get; private set; }
        internal INode TermMapNode { get; private set; }

        internal TermMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            TriplesMapNode = triplesMapNode;
            TermMapNode = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(TriplesMapNode, R2RMLMappings.CreateUriNode(RrSubjectMapProperty), TermMapNode);
        }

        #region Implementation of ITermMapConfiguration
        #endregion

        #region Implementation of ISubjectMapConfiguration

        public ISubjectMapConfiguration AddClass(Uri classIri)
        {
            R2RMLMappings.Assert(
                TermMapNode, 
                R2RMLMappings.CreateUriNode(RrClassClass), 
                R2RMLMappings.CreateUriNode(classIri));

            return this;
        }

        public Uri[] ClassIris
        {
            get
            {
                var classes = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(RrClassClass));
                return classes.Select(triple => ((IUriNode)triple.Object).Uri).ToArray();
            }
        }

        #endregion
    }
}
