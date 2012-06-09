using System;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class SubjectMapConfiguration : TermMapConfiguration, ISubjectMapConfiguration
    {
        public SubjectMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

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

        #region Overrides of TermMapConfiguration

        public override ITermMapConfiguration IsBlankNode()
        {
            throw new NotImplementedException();
        }

        public override ITermMapConfiguration IsIRI()
        {
            throw new NotImplementedException();
        }

        public override ITermMapConfiguration IsLiteral()
        {
            throw new InvalidTriplesMapException("Subject map cannot be of term type rr:Literal");
        }

        #endregion
    }
}