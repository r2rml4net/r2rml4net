using System;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Fluent configuration of subject map backed by a DotNetRDF graph (see <see cref="ISubjectMapConfiguration"/>)
    /// </summary>
    public class SubjectMapConfiguration : TermMapConfiguration, ISubjectMapConfiguration
    {
        internal SubjectMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
            R2RMLMappings.Assert(TriplesMapNode, R2RMLMappings.CreateUriNode(RrSubjectMapProperty), TermMapNode);
        }

        #region Implementation of ISubjectMapConfiguration

        /// <summary>
        /// <see cref="ISubjectMapConfiguration.AddClass"/>
        /// </summary>
        public ISubjectMapConfiguration AddClass(Uri classIri)
        {
            R2RMLMappings.Assert(
                TermMapNode,
                R2RMLMappings.CreateUriNode(RrClassClass),
                R2RMLMappings.CreateUriNode(classIri));

            return this;
        }

        /// <summary>
        /// <see cref="ISubjectMapConfiguration.ClassIris"/>
        /// </summary>
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

        /// <summary>
        /// <see cref="ITermMapConfiguration.TermTypeIRI"/>
        /// </summary>
        public override Uri TermTypeIRI
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Sets this subject node to produce blank nodes
        /// </summary>
        public override ITermMapConfiguration IsBlankNode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets this subject node to produce IRI nodes
        /// </summary>
        public override ITermMapConfiguration IsIRI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Throws exception
        /// </summary>
        public override ITermMapConfiguration IsLiteral()
        {
            throw new InvalidTriplesMapException("Subject map cannot be of term type rr:Literal");
        }

        #endregion
    }
}