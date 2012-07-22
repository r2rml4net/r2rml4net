using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    class W3CPredicateObjectMapProcessor : MapProcessorBase, IPredicateObjectMapProcessor
    {
        private readonly IRDFTermGenerator _termGenerator;

        public W3CPredicateObjectMapProcessor(IRDFTermGenerator termGenerator, IRdfHandler rdfHandler)
            : base(rdfHandler)
        {
            _termGenerator = termGenerator;
        }

        #region Implementation of IPredicateObjectMapProcessor

        public void ProcessPredicateObjectMap(INode subject, IPredicateObjectMap predicateObjectMap, IEnumerable<IUriNode> subjectGraphs, IDataRecord logicalRow)
        {
            var predicates = (from predicateMap in predicateObjectMap.PredicateMaps
                              select _termGenerator.GenerateTerm<IUriNode>(predicateMap, logicalRow)).ToArray();
            var objects = (from objectMap in predicateObjectMap.ObjectMaps
                           select _termGenerator.GenerateTerm<INode>(objectMap, logicalRow)).ToArray();
            IEnumerable<IUriNode> graphs = (from graphMap in predicateObjectMap.GraphMaps
                                            select _termGenerator.GenerateTerm<IUriNode>(graphMap, logicalRow)).ToList();
            var subjectGraphsLocal = subjectGraphs.ToArray();

            if (!graphs.Any())
            {
                graphs = new[] {CreateUriNode(new Uri(RrDefaultgraph))};
            }

            AddTriplesToDataSet(subject, predicates, objects, graphs.Union(subjectGraphsLocal).ToList());
        }

        #endregion
    }
}