#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System.Linq;
using System.Collections.Generic;
using TCode.r2rml4net.Exceptions;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping.Fluent
{
    class PredicateObjectMapConfiguration : BaseConfiguration, IPredicateObjectMapConfiguration
    {
        private readonly IList<ObjectMapConfiguration> _objectMaps = new List<ObjectMapConfiguration>();
        private readonly IList<RefObjectMapConfiguration> _refObjectMaps = new List<RefObjectMapConfiguration>();
        private readonly IList<PredicateMapConfiguration> _predicateMaps = new List<PredicateMapConfiguration>();
        private readonly IList<GraphMapConfiguration> _graphMaps = new List<GraphMapConfiguration>();

        internal PredicateObjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraph r2RMLMappings)
            : this(parentTriplesMap, r2RMLMappings, r2RMLMappings.CreateBlankNode())
        {
        }

        internal PredicateObjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraph r2RMLMappings, INode node)
            : base(parentTriplesMap, r2RMLMappings, node)
        {
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

        public IGraphMapConfiguration CreateGraphMap()
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
            CreateSubMaps(R2RMLUris.RrGraphMapPropety, (graph, node) => new GraphMapConfiguration(TriplesMap, this, graph, node), _graphMaps);
            CreateSubMaps(R2RMLUris.RrPredicateMapPropety, (graph, node) => new PredicateMapConfiguration(TriplesMap, this, graph, node), _predicateMaps);
            CreateObjectMaps();
            CreateRefObjectMaps();
        }

        private void CreateObjectMaps()
        {
            var query = new SparqlParameterizedString(@"PREFIX rr: <http://www.w3.org/ns/r2rml#>
SELECT ?predObj ?objectMap  
WHERE
{ 
    @triplesMap rr:predicateObjectMap ?predObj .
    ?predObj rr:objectMap ?objectMap .
    optional { ?objectMap rr:parentTriplesMap ?triplesMap }
    FILTER(!BOUND(?triplesMap))
}");
            query.SetParameter("triplesMap", TriplesMap.Node);
            query.SetParameter("objectMapProperty", R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty));
            query.SetParameter("parentTriplesMap", R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty));
            var resultSet = (SparqlResultSet) R2RMLMappings.ExecuteQuery(query);
            
            foreach (var result in resultSet.Where(result => result["predObj"].Equals(Node)))
            {
                var subConfiguration = new ObjectMapConfiguration(TriplesMap, this, R2RMLMappings, result["objectMap"]);
                subConfiguration.RecursiveInitializeSubMapsFromCurrentGraph();
                _objectMaps.Add(subConfiguration);
            }
        }

        private void CreateRefObjectMaps()
        {
            var query = new SparqlParameterizedString(@"PREFIX rr: <http://www.w3.org/ns/r2rml#>
SELECT ?objectMap ?triplesMap ?predObjectMap
WHERE 
{ 
    @childTriplesMap rr:predicateObjectMap ?predObjectMap .
    ?predObjectMap rr:objectMap ?objectMap . 
    ?objectMap rr:parentTriplesMap ?triplesMap .
}");
            query.SetParameter("childTriplesMap", TriplesMap.Node);
            var resultSet = (SparqlResultSet) R2RMLMappings.ExecuteQuery(query);

            foreach (var result in resultSet.Where(result => result["predObjectMap"].Equals(Node)))
            {
                ITriplesMap referencedTriplesMap =
                    TriplesMap.R2RMLConfiguration.TriplesMaps.SingleOrDefault(tMap => result.Value("triplesMap").Equals(tMap.Node));

                if(referencedTriplesMap == null)
                    throw new InvalidMapException(string.Format("Triples map {0} not found. It must be added before creating ref object map", result.Value("triplesMap")));

                var subConfiguration = new RefObjectMapConfiguration(this, TriplesMap, referencedTriplesMap, R2RMLMappings, result["objectMap"]);
                subConfiguration.RecursiveInitializeSubMapsFromCurrentGraph();
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