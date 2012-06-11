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

        /// <summary>
        /// </summary>
        protected TermMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            TriplesMapNode = triplesMapNode;
            TermMapNode = R2RMLMappings.CreateBlankNode();
        }

        #region Implementation of ITermMapConfiguration

        /// <summary>
        /// <see cref="ITermMapConfiguration.TermType"/>
        /// </summary>
        public ITermTypeConfiguration TermType
        {
            get { return this; }
        }

        public ITermTypeConfiguration IsConstantValued(Uri uri)
        {
            if (R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, CreateConstantPropertyNode()).Any())
                throw new InvalidTriplesMapException("Term map can have at most one constant value");

            CheckRelationWithParentMap(true);

            R2RMLMappings.Assert(TriplesMapNode, CreateConstantPropertyNode(), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        public void IsColumnValued(string columnName)
        {
            CheckRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(RrColumnProperty), R2RMLMappings.CreateLiteralNode(columnName));
        }

        #endregion

        #region Implementation of ITermTypeConfiguration

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsBlankNode"/>
        /// </summary>
        public virtual ITermMapConfiguration IsBlankNode()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(RrTermTypeProperty), R2RMLMappings.CreateUriNode(RrBlankNode));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsIRI"/>
        /// </summary>
        public virtual ITermMapConfiguration IsIRI()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(RrTermTypeProperty), R2RMLMappings.CreateUriNode(RrIRI));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsLiteral"/>
        /// </summary>
        public virtual ITermMapConfiguration IsLiteral()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(RrTermTypeProperty), R2RMLMappings.CreateUriNode(RrLiteral));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.URI"/>
        /// </summary>
        public virtual Uri URI
        {
            get
            {
                return ExplicitTermType ?? R2RMLMappings.CreateUriNode(RrIRI).Uri;
            }
        }

        /// <summary>
        /// Gets explicitly set
        /// </summary>
        private Uri ExplicitTermType
        {
            get
            {
                var termTypeNodes = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode,
                                                                                 R2RMLMappings.CreateUriNode(RrTermTypeProperty)).ToArray();

                if (termTypeNodes.Length > 1)
                    throw new InvalidTriplesMapException(string.Format("TermMap has {0} (should be zero or one)", termTypeNodes.Length));

                if (termTypeNodes.Length == 1)
                {
                    IUriNode termTypeNode = termTypeNodes[0].Object as IUriNode;
                    if (termTypeNode == null)
                        throw new InvalidTriplesMapException("Term type must be an IRI");

                    return termTypeNode.Uri;
                }

                return null;
            }
        }

        ///<summary>
        /// Checks wheather term type is already set
        /// </summary>
        /// <exception cref="InvalidTriplesMapException" />
        protected void AssertTermTypeNotSet()
        {
            if(ExplicitTermType != null)
                throw new InvalidTriplesMapException("Term type already set");
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="IUriNode"/> for the shorcut property as described on http://www.w3.org/TR/r2rml/#constant
        /// </summary>
        protected internal abstract IUriNode CreateConstantPropertyNode();
        /// <summary>
        /// Returns a term map property
        /// </summary>
        /// <returns>one of the following: rr:subjectMap, rr:objectMap, rr:propertyMap or rr:graphMap</returns>
        protected internal abstract IUriNode CreateMapPropertyNode();

        protected void CheckRelationWithParentMap(bool useShortcutProperty = false)
        {
            var containsMapPropertyTriple = R2RMLMappings.ContainsTriple(new Triple(TriplesMapNode, CreateMapPropertyNode(), TermMapNode));
            var shortcutPropertyTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TriplesMapNode, CreateConstantPropertyNode());

            if((containsMapPropertyTriple && !useShortcutProperty) || (shortcutPropertyTriples.Any() && useShortcutProperty))
                throw new InvalidTriplesMapException(string.Format("Cannot use {0} and {1} properties simultanously", CreateConstantPropertyNode(), CreateMapPropertyNode()));

            if(!useShortcutProperty)
            {
                R2RMLMappings.Assert(TriplesMapNode, CreateMapPropertyNode(), TermMapNode);
            }
        }
    }
}
