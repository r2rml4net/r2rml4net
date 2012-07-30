using System;
using System.Linq;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration
    {
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<RefObjectMapConfiguration> _refObjectMaps = new List<RefObjectMapConfiguration>();
        private readonly IList<PredicateMapConfiguration> _predicateMaps = new List<PredicateMapConfiguration>();
        private readonly IList<GraphMapConfiguration> _graphMaps = new List<GraphMapConfiguration>();

        internal PredicateObjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraph r2RMLMappings)
            : base(parentTriplesMap, r2RMLMappings)
        {
            Node = R2RMLMappings.CreateBlankNode();
            R2RMLMappings.Assert(parentTriplesMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrPredicateObjectMapPropety), Node);
        }

        #region Implementation of IPredicateObjectMapConfiguration

        public IObjectMapConfiguration CreateObjectMap()
        {
            var objectMap = new ObjectMapConfiguration(TriplesMap, this, R2RMLMappings);
            _objectMaps.Add(objectMap);
            return objectMap;
        }

        public ITermMapConfiguration CreatePredicateMap()
        {
            var propertyMap = new PredicateMapConfiguration(TriplesMap, this, R2RMLMappings);
            _predicateMaps.Add(propertyMap);
            return propertyMap;
        }

        public IGraphMap CreateGraphMap()
        {
            var graphMap = new GraphMapConfiguration(TriplesMap, this, R2RMLMappings);
            _graphMaps.Add(graphMap);
            return graphMap;
        }

        public IRefObjectMapConfiguration CreateRefObjectMap(ITriplesMapConfiguration referencedTriplesMap)
        {
            var refObjectMap = new RefObjectMapConfiguration(this, TriplesMap, referencedTriplesMap, R2RMLMappings);
            _refObjectMaps.Add(refObjectMap);
            return refObjectMap;
        }

        #endregion

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            CreateSubMaps(R2RMLUris.RrGraphMapPropety, graph => new GraphMapConfiguration(TriplesMap, this, graph), _graphMaps);
            CreateSubMaps(R2RMLUris.RrPredicateMapPropety, graph => new PredicateMapConfiguration(TriplesMap, this, graph), _predicateMaps);
            CreateObjectMaps();
            CreateRefObjectMaps();
        }

        private void CreateObjectMaps()
        {
            var query = new SparqlParameterizedString(@"PREFIX rr: <http://www.w3.org/ns/r2rml#>
SELECT ?predObj ?objectMap  
WHERE
{ 
    { 
        @triplesMap rr:predicateObjectMap ?predObj .
        ?predObj rr:objectMap ?objectMap 
        FILTER NOT EXISTS 
        {
            ?objectMap rr:parentTriplesMap ?triplesMap 
        } 
    } 
}");
            query.SetParameter("triplesMap", TriplesMap.Node);
            query.SetParameter("objectMapProperty", R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty));
            query.SetParameter("parentTriplesMap", R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty));
            var resultSet = (SparqlResultSet) R2RMLMappings.ExecuteQuery(query);

            foreach (var result in resultSet.Where(result => result["predObj"].Equals(Node)))
            {
                var subConfiguration = new ObjectMapConfiguration(TriplesMap, this, R2RMLMappings);
                subConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(result.Value("objectMap"));
                _objectMaps.Add(subConfiguration);
            }
        }

        private void CreateRefObjectMaps()
        {
            var query = new SparqlParameterizedString(@"PREFIX rr: <http://www.w3.org/ns/r2rml#>
SELECT ?objectMap ?triplesMap 
WHERE 
{ 
    @childTriplesMap rr:predicateObjectMap @predicateObjectMap .
    @predicateObjectMap rr:objectMap ?objectMap . 
    ?objectMap rr:parentTriplesMap ?triplesMap .
}");
            query.SetParameter("childTriplesMap", TriplesMap.Node);
            query.SetParameter("predicateObjectMap", Node);
            query.SetParameter("predicateObjectMap", Node);
            var resultSet = (SparqlResultSet) R2RMLMappings.ExecuteQuery(query);

            foreach (var result in resultSet)
            {
                ITriplesMap referencedTriplesMap =
                    TriplesMap.R2RMLConfiguration.TriplesMaps.SingleOrDefault(tMap => result.Value("triplesMap").Equals(tMap.Node));

                if(referencedTriplesMap == null)
                    throw new InvalidTriplesMapException(string.Format("Triples map {0} not found. It must be added before creating ref object map", result.Value("triplesMap")));

                var subConfiguration = new RefObjectMapConfiguration(this, TriplesMap, referencedTriplesMap, R2RMLMappings);
                subConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(result.Value("objectMap"));
                _refObjectMaps.Add(subConfiguration);
            }
        }

        #endregion

        #region Implementation of IPredicateObjectMap

        public IEnumerable<IObjectMap> ObjectMaps
        {
            get { return _objectMaps; }
        }

        public IEnumerable<IRefObjectMap> RefObjectMaps
        {
            get { return _refObjectMaps; }
        }

        public IEnumerable<IPredicateMap> PredicateMaps
        {
            get { return _predicateMaps; }
        }

        public IEnumerable<IGraphMap> GraphMaps
        {
            get { return _graphMaps; }
        }

        #endregion
    }
}