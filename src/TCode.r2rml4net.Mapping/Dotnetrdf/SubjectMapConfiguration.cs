using System;
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Dotnetrdf
{
    /// <summary>
    /// Fluent configuration of subject map backed by a DotNetRDF graph (see <see cref="ISubjectMapConfiguration"/>)
    /// </summary>
    internal class SubjectMapConfiguration : TermMapConfiguration, ISubjectMapConfiguration, INonLiteralTermMapConfigutarion, ISubjectMap
    {
        private readonly IList<GraphMapConfiguration> _graphMaps = new List<GraphMapConfiguration>();

        internal SubjectMapConfiguration(INode parentMapNode, IGraph r2RMLMappings) : base(parentMapNode, r2RMLMappings)
        {
        }

        #region Implementation of ISubjectMapConfiguration

        /// <summary>
        /// <see cref="ISubjectMapConfiguration.AddClass"/>
        /// </summary>
        public ISubjectMapConfiguration AddClass(Uri classIri)
        {
            // create SubjectMap - TriplesMap relation if no class has been added
            if(ClassIris.Length == 0)
                CreateParentMapRelation();

            R2RMLMappings.Assert(
                TermMapNode,
                R2RMLMappings.CreateUriNode(R2RMLUris.RrClassProperty),
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
                var classes = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrClassProperty));
                return classes.Select(triple => ((IUriNode)triple.Object).Uri).ToArray();
            }
        }

        public IGraphMap CreateGraphMap()
        {
            var graphMap = new GraphMapConfiguration(TermMapNode, R2RMLMappings);
            _graphMaps.Add(graphMap);
            return graphMap;
        }

        #endregion

        #region Overrides of TermMapConfiguration

        /// <summary>
        /// Returns rr:subjectMap
        /// </summary>
        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrSubjectMapProperty);
        }

        #endregion

        #region Implementation of ISubjectMap

        /// <summary>
        /// <see cref="ISubjectMap.Subject"/>
        /// </summary>
        public Uri Subject
        {
            get { return ConstantValue; }
        }

        #endregion
    }
}