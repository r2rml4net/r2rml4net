using System;
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Base for DotNetRDF-backed fluent R2RML configuration
    /// </summary>
    public abstract class BaseConfiguration : IMapBase
    {
        private readonly ITriplesMapConfiguration _parentTriplesMap;

        private const string ShortcutSubmapsReplaceSparql = @"PREFIX rr: <http://www.w3.org/ns/r2rml#>
DELETE { ?map rr:graph ?value . }
INSERT { ?map rr:graphMap [ rr:constant ?value ] . }
WHERE { ?map rr:graph ?value } ;

DELETE { ?map rr:object ?value . }
INSERT { ?map rr:objectMap [ rr:constant ?value ] . }
WHERE { ?map rr:object ?value } ;

DELETE { ?map rr:predicate ?value . }
INSERT { ?map rr:predicateMap [ rr:constant ?value ] . }
WHERE { ?map rr:predicate ?value } ;

DELETE { ?map rr:subject ?value . }
INSERT { ?map rr:subjectMap [ rr:constant ?value ] . }
WHERE { ?map rr:subject ?value }";

        /// <summary>
        /// DotNetRDF graph containing the R2RML mappings
        /// </summary>
        protected internal IGraph R2RMLMappings { get; private set; }

        /// <summary>
        /// Constructor used by <see cref="R2RMLConfiguration"/>
        /// </summary>
        /// <param name="baseUri">R2RML graph's base URI</param>
        protected BaseConfiguration(Uri baseUri)
        {
            R2RMLMappings = new Graph { BaseUri = baseUri };
            EnsurePrefixes();
        }

        /// <summary>
        /// Constructor used by <see cref="TriplesMapConfiguration"/>
        /// </summary>
        protected BaseConfiguration(IGraph existingMappingsGraph)
        {
            R2RMLMappings = existingMappingsGraph;
            EnsureNoShortcutSubmaps();
            EnsurePrefixes();
        }

        /// <summary>
        /// Constructor used by implementations other than <see cref="R2RMLConfiguration"/> and <see cref="TriplesMapConfiguration"/>
        /// </summary>
        protected BaseConfiguration(ITriplesMapConfiguration triplesMap, IGraph existingMappingsGraph)
            : this(existingMappingsGraph)
        {
            _parentTriplesMap = triplesMap;
        }

        #region Implementation of IMapBase

        public abstract INode Node { get; }

        #endregion

        private void EnsurePrefixes()
        {
            if (!R2RMLMappings.NamespaceMap.HasNamespace("rr"))
                R2RMLMappings.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
        }

        /// <summary>
        /// Reads all maps contained in the current configuration and creates configuration objects
        /// </summary>
        /// <remarks>Used in loading configuration from an exinsting graph</remarks>
        protected internal virtual void RecursiveInitializeSubMapsFromCurrentGraph(INode currentNode)
        {
            InitializeSubMapsFromCurrentGraph();
        }

        /// <summary>
        /// Implemented in child classes should create submaps and for each of the run the <see cref="BaseConfiguration.RecursiveInitializeSubMapsFromCurrentGraph"/> method
        /// </summary>
        protected abstract void InitializeSubMapsFromCurrentGraph();

        /// <summary>
        /// Overriden in child classes should change shortcut properties to maps
        /// </summary>
        /// <example>{ [] rr:graph ex:instance } should become { [] rr:graphMap [ rr:constant ex:instance ] }</example>
        protected void EnsureNoShortcutSubmaps()
        {
            TripleStore store = new TripleStore();
            store.Add(R2RMLMappings);

            var dataset = new InMemoryDataset(store, R2RMLMappings.BaseUri);
            ISparqlUpdateProcessor processor = new LeviathanUpdateProcessor(dataset);
            var updateParser = new SparqlUpdateParser();

            processor.ProcessCommandSet(updateParser.ParseFromString(ShortcutSubmapsReplaceSparql));
        }

        protected void CreateSubMaps<TConfiguration>(string property, Func<IGraph, TConfiguration> createSubConfiguration, IList<TConfiguration> subMaps)
            where TConfiguration : BaseConfiguration
        {
            var mapPropety = R2RMLMappings.CreateUriNode(property);
            var triples = R2RMLMappings.GetTriplesWithSubjectPredicate(this.Node, mapPropety);

            foreach (var triple in triples.ToArray())
            {
                var subConfiguration = createSubConfiguration(R2RMLMappings);
                subConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(triple.Object);
                subMaps.Add(subConfiguration);
            }
        }

        protected internal virtual ITriplesMapConfiguration ParentTriplesMap
        {
            get { return _parentTriplesMap; }
        }
    }
}
