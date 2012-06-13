using System;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Base fluent configuration of term maps (subject maps, predicate maps, graph maps or object maps) 
    /// backed by a DotNetRDF graph (see <see cref="ITermMapConfiguration"/>)
    /// </summary>
    public abstract class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ITermTypeConfiguration, ITermMap, ITermType
    {
        /// <summary>
        /// The parent node for the current <see cref="TermMapNode"/>.
        /// </summary>
        /// <remarks>
        /// Depending on the type the parent can be a triples map (for property-object maps, subject maps), 
        /// property-object map (for object maps, property maps and graph maps)
        /// or subject map (for graph maps)
        /// </remarks>
        protected internal INode ParentMapNode { get; private set; }
        internal INode TermMapNode { get; private set; }

        /// <summary>
        /// </summary>
        protected TermMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            ParentMapNode = triplesMapNode;
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

        /// <summary>
        /// <see cref="ITermMapConfiguration.IsConstantValued"/>
        /// </summary>
        public ITermTypeConfiguration IsConstantValued(Uri uri)
        {
            if (ConstantValue != null)
                throw new InvalidTriplesMapException("Term map can have at most one constant value");

            CheckRelationWithParentMap(true);

            R2RMLMappings.Assert(ParentMapNode, CreateConstantPropertyNode(), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        /// <summary>
        /// <see cref="ITermMapConfiguration.IsTemplateValued"/>
        /// </summary>
        public ITermTypeConfiguration IsTemplateValued(string template)
        {
            IUriNode templateProperty = R2RMLMappings.CreateUriNode(UrisHelper.RrTemplateProperty);
            if (R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, templateProperty).Any())
                throw new InvalidTriplesMapException("Term map can have at most one template");

            CheckRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, templateProperty, R2RMLMappings.CreateLiteralNode(template));
            return this;
        }

        #endregion

        #region Implementation of ITermTypeConfiguration

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsBlankNode"/>
        /// </summary>
        public virtual ITermMapConfiguration IsBlankNode()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrTermTypeProperty), R2RMLMappings.CreateUriNode(UrisHelper.RrBlankNode));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsIRI"/>
        /// </summary>
        public virtual ITermMapConfiguration IsIRI()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrTermTypeProperty), R2RMLMappings.CreateUriNode(UrisHelper.RrIRI));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsLiteral"/>
        /// </summary>
        public virtual ITermMapConfiguration IsLiteral()
        {
            throw new InvalidTriplesMapException("Only object map can be of term type rr:Literal");
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.URI"/>
        /// </summary>
        public virtual Uri URI
        {
            get
            {
                return ExplicitTermType ?? R2RMLMappings.CreateUriNode(UrisHelper.RrIRI).Uri;
            }
        }

        /// <summary>
        /// Gets explicitly set
        /// </summary>
        protected Uri ExplicitTermType
        {
            get
            {
                var termTypeNodes = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode,
                                                                                 R2RMLMappings.CreateUriNode(UrisHelper.RrTermTypeProperty)).ToArray();

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
            if (ExplicitTermType != null)
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
        /// <summary>
        /// Verifies that the term map doesn't have both shortcut and "full" property set
        /// </summary>
        /// <param name="useShortcutProperty">if false, will create a relation with <see cref="TermMapNode"/> and <see cref="ParentMapNode"/>. 
        /// This relation is not needed when using constant value shortcut. If the latter is true, the value is connected directly to <see cref="ParentMapNode"/>
        /// using the property created by <see cref="CreateConstantPropertyNode"/></param>
        /// <example>An example invalid graph (object map): [] rr:subject "value"; rr:subjectMap [ rr:column "ColumnName" ; ] .</example>
        protected void CheckRelationWithParentMap(bool useShortcutProperty = false)
        {
            var containsMapPropertyTriple = R2RMLMappings.ContainsTriple(new Triple(ParentMapNode, CreateMapPropertyNode(), TermMapNode));
            var shortcutPropertyTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, CreateConstantPropertyNode()).ToArray();

            if ((containsMapPropertyTriple && shortcutPropertyTriples.Any())
                || (containsMapPropertyTriple && useShortcutProperty)
                || (shortcutPropertyTriples.Any() && !useShortcutProperty))
                throw new InvalidTriplesMapException(string.Format("Cannot use {0} and {1} properties simultanously", CreateConstantPropertyNode(), CreateMapPropertyNode()));

            if (!useShortcutProperty)
            {
                CreateParentMapRelation();
            }
        }

        /// <summary>
        /// Creates relation with parent map (using rr:subjectMap, rr:objectMap, rr:propertyMap or rr:graphMap)
        /// </summary>
        protected void CreateParentMapRelation()
        {
            R2RMLMappings.Assert(ParentMapNode, CreateMapPropertyNode(), TermMapNode);
        }

        /// <summary>
        /// <see cref="INonLiteralTermMapConfigutarion.IsColumnValued"/>
        /// </summary>
        public void IsColumnValued(string columnName)
        {
            if(ColumnName != null)
                throw new InvalidTriplesMapException("Term map can have only one column name");

            CheckRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrColumnProperty), R2RMLMappings.CreateLiteralNode(columnName));
        }

        #region Implementation of ITermMap

        public string ColumnName
        {
            get
            {
                var columnTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrColumnProperty)).ToArray();

                if (columnTriples.Length == 1)
                    if (columnTriples[0].Object is ILiteralNode)
                        return columnTriples[0].Object.ToString();
                    else
                        throw new InvalidTriplesMapException("Term map column must be a literal");

                if (columnTriples.Length == 0)
                    return null;

                throw new InvalidTriplesMapException(
                    string.Format("Term map contains multiple column names:\r\n{0}",
                                  string.Join("\r\n", columnTriples.Select(triple => triple.Object.ToString()))));
            }
        }

        public string Template
        {
            get
            {
                var templateTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrTemplateProperty)).ToArray();

                if (templateTriples.Length == 1)
                    if (templateTriples[0].Object is ILiteralNode)
                        return templateTriples[0].Object.ToString();
                    else
                        throw new InvalidTriplesMapException("Term map template must be a literal");

                if (templateTriples.Length == 0)
                    return null;

                throw new InvalidTriplesMapException(
                    string.Format("Term map contains multiple templates:\r\n{0}",
                                  string.Join("\r\n", templateTriples.Select(triple => triple.Object.ToString()))));
            }
        }

        public Uri ConstantValue
        {
            get
            {
                var shortcutTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, CreateConstantPropertyNode()).ToArray();
                var constantTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, CreateMapPropertyNode()).ToArray();

                if (shortcutTriples.Length == 0 && constantTriples.Length == 0)
                    return null;

                if (shortcutTriples.Any() && constantTriples.Any() || shortcutTriples.Length > 1 || constantTriples.Length > 1)
                    throw new InvalidTriplesMapException("Term map can have at most one constant value");

                IUriNode uriNode = null;
                if (shortcutTriples.Any())
                    uriNode = shortcutTriples[0].Object as IUriNode;
                else if (constantTriples.Any())
                    uriNode = constantTriples[0].Object as IUriNode;

                if (uriNode == null)
                    throw new InvalidTriplesMapException("Constant value must be a URI");

                return uriNode.Uri;
            }
        }

        ITermType ITermMap.TermType
        {
            get { return this; }
        }

        #endregion
    }
}
