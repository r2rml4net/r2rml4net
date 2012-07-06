using System;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Base fluent configuration of term maps (subject maps, predicate maps, graph maps or object maps) 
    /// backed by a DotNetRDF graph (see <see cref="ITermMapConfiguration"/>)
    /// </summary>
    public abstract class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ITermTypeConfiguration, ITermMap, ITermType
    {
        private const string ShortcutReplaceSparqlFormat = @"PREFIX rr: <http://www.w3.org/ns/r2rml#>
DELETE {{ ?map <{0}> ?value . }}
INSERT {{ ?map <{1}> [ rr:constant ?value ] . }}
WHERE {{ }}";

        /// <summary>
        /// The parent node for the current <see cref="TermMapNode"/>.
        /// </summary>
        /// <remarks>
        /// Depending on the type the parent can be a triples map (for property-object maps, subject maps), 
        /// property-object map (for object maps, property maps and graph maps)
        /// or subject map (for graph maps)
        /// </remarks>
        protected internal INode ParentMapNode { get; private set; }
        protected internal INode TermMapNode { get; protected set; }

        /// <summary>
        /// </summary>
        protected TermMapConfiguration(INode parentMapNode, IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            ParentMapNode = parentMapNode;
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

            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrConstantProperty), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        /// <summary>
        /// <see cref="ITermMapConfiguration.IsTemplateValued"/>
        /// </summary>
        public ITermTypeConfiguration IsTemplateValued(string template)
        {
            IUriNode templateProperty = R2RMLMappings.CreateUriNode(R2RMLUris.RrTemplateProperty);
            if (R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, templateProperty).Any())
                throw new InvalidTriplesMapException("Term map can have at most one template");

            EnsureRelationWithParentMap();

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
            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty), R2RMLMappings.CreateUriNode(R2RMLUris.RrBlankNode));
            return this;
        }

        /// <summary>
        /// <see cref="ITermTypeConfiguration.IsIRI"/>
        /// </summary>
        public virtual ITermMapConfiguration IsIRI()
        {
            AssertTermTypeNotSet();
            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty), R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI));
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
        /// <see cref="ITermType.URI"/>
        /// </summary>
        public virtual Uri URI
        {
            get
            {
                return ExplicitTermType ?? R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI).Uri;
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
                                                                                 R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty)).ToArray();

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

        #region Implementation of ITermMap

        /// <summary>
        /// <see cref="ITermMap.ColumnName"/>
        /// </summary>
        public string ColumnName
        {
            get { return GetSingleLiteralValueForPredicate(R2RMLMappings.CreateUriNode(R2RMLUris.RrColumnProperty)); }
        }

        /// <summary>
        /// <see cref="ITermMap.Template"/>
        /// </summary>
        public string Template
        {
            get { return GetSingleLiteralValueForPredicate(R2RMLMappings.CreateUriNode(R2RMLUris.RrTemplateProperty)); }
        }

        /// <summary>
        /// Gets the constant URI value for this term map
        /// </summary>
        /// <remarks>Read more on http://www.w3.org/TR/r2rml/#constant</remarks>
        protected internal Uri ConstantValue
        {
            get { return GetSingleUriValueForPredicate(R2RMLMappings.CreateUriNode(R2RMLUris.RrConstantProperty)); }
        }

        ITermType ITermMap.TermType
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Overrides of BaseConfiguration
        
        protected internal override void RecursiveInitializeSubMapsFromCurrentGraph()
        {
            EnsureGraphHasNoShortcutProperties();
            base.RecursiveInitializeSubMapsFromCurrentGraph();
        } 

        #endregion

        /// <summary>
        /// Overriden in child classes should change shortcut properties to maps
        /// </summary>
        /// <example>{ [] rr:graph ex:instance } should become { [] rr:graphMap [ rr:constant ex:instance ] }</example>
        private void EnsureGraphHasNoShortcutProperties()
        {
            var dataset = new InMemoryDataset(true);
            dataset.AddGraph(R2RMLMappings);
            ISparqlUpdateProcessor processor = new LeviathanUpdateProcessor(dataset);
            var updateParser = new SparqlUpdateParser();

            var rrShortcutProperty = CreateShortcutPropertyNode();
            var rrMapProperty = CreateMapPropertyNode();
            var sparqlQuery = string.Format(ShortcutReplaceSparqlFormat, rrShortcutProperty, rrMapProperty);

            processor.ProcessCommandSet(updateParser.ParseFromString(sparqlQuery));
        }

        /// <summary>
        /// Returns a term map property
        /// </summary>
        /// <returns>one of the following: rr:subjectMap, rr:objectMap, rr:propertyMap or rr:graphMap</returns>
        protected internal abstract IUriNode CreateMapPropertyNode();

        /// <summary>
        /// Returns a constant term shortcut property
        /// </summary>
        /// <returns>one of the following: rr:subject, rr:object, rr:property or rr:graph</returns>
        protected internal abstract IUriNode CreateShortcutPropertyNode();

        /// <summary>
        /// Ensures that <see cref="ParentMapNode"/> and this <see cref="TermMapNode"/> are 
        /// connected with the appropriate property
        /// </summary>
        /// <remarks>The two nodes will be connected by return value of <see cref="CreateMapPropertyNode"/> method</remarks>
        protected void EnsureRelationWithParentMap()
        {
            var containsMapPropertyTriple = R2RMLMappings.ContainsTriple(new Triple(ParentMapNode, CreateMapPropertyNode(), TermMapNode));

            if (!containsMapPropertyTriple)
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
            if (ColumnName != null)
                throw new InvalidTriplesMapException("Term map can have only one column name");

            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrColumnProperty), R2RMLMappings.CreateLiteralNode(columnName));
        }

        /// <summary>
        /// Gets a single literal object value for <see cref="TermMapNode"/> ans <paramref name="predicate"/> predicate
        /// </summary>
        /// <exception cref="InvalidTriplesMapException">if multiple values found or object is not a literal</exception>
        protected string GetSingleLiteralValueForPredicate(IUriNode predicate)
        {
            var triplesForPredicate = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, predicate).ToArray();

            if (triplesForPredicate.Length == 1)
                if (triplesForPredicate[0].Object is ILiteralNode)
                    return triplesForPredicate[0].Object.ToString();
                else
                    throw new InvalidTriplesMapException(string.Format("Term map value for {0} must be a literal", predicate.Uri));

            if (triplesForPredicate.Length == 0)
                return null;

            throw new InvalidTriplesMapException(
                string.Format("Term map contains multiple values for {1}:\r\n{0}",
                              string.Join("\r\n", triplesForPredicate.Select(triple => triple.Object.ToString())),
                              predicate.Uri));
        }

        /// <summary>
        /// Gets a single URI object value for <see cref="TermMapNode"/> ans <paramref name="predicate"/> predicate
        /// </summary>
        /// <exception cref="InvalidTriplesMapException">if multiple values found or object is not a URI</exception>
        protected Uri GetSingleUriValueForPredicate(IUriNode predicate)
        {
            var triplesForPredicate = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, predicate).ToArray();

            if (triplesForPredicate.Length == 1)
                if (triplesForPredicate[0].Object is IUriNode)
                    return ((IUriNode)triplesForPredicate[0].Object).Uri;
                else
                    throw new InvalidTriplesMapException(string.Format("Term map value for {0} must be a URI", predicate.Uri));

            if (triplesForPredicate.Length == 0)
                return null;

            throw new InvalidTriplesMapException(
                string.Format("Term map contains multiple values for {1}:\r\n{0}",
                              string.Join("\r\n", triplesForPredicate.Select(triple => triple.Object.ToString())),
                              predicate.Uri));
        }
    }
}
