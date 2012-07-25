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
        public W3CPredicateObjectMapProcessor(IRDFTermGenerator termGenerator, IRdfHandler rdfHandler)
            : base(termGenerator, rdfHandler)
        {
        }

        #region Implementation of IPredicateObjectMapProcessor

        public void ProcessPredicateObjectMap(INode subject, IPredicateObjectMap predicateObjectMap, IEnumerable<IUriNode> subjectGraphs, IDataRecord logicalRow)
        {
            var predicates = (from predicateMap in predicateObjectMap.PredicateMaps
                              select TermGenerator.GenerateTerm<IUriNode>(predicateMap, logicalRow)).ToArray();
            var objects = (from objectMap in predicateObjectMap.ObjectMaps
                           select TermGenerator.GenerateTerm<INode>(objectMap, logicalRow)).ToArray();
            var graphs = (from graphMap in predicateObjectMap.GraphMaps
                          select TermGenerator.GenerateTerm<IUriNode>(graphMap, logicalRow)).ToArray();
            var subjectGraphsLocal = subjectGraphs.ToArray();

            AddTriplesToDataSet(subject, predicates, objects, graphs.Union(subjectGraphsLocal).ToList());
        }

        #endregion
    }
}