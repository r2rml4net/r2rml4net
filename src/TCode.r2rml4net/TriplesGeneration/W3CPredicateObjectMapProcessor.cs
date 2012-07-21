using System.Collections.Generic;
using System.Data;
using System.Linq;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    class W3CPredicateObjectMapProcessor : IPredicateObjectMapProcessor
    {
        private readonly IRDFTermGenerator _termGenerator;
        private readonly IRdfHandler _rdfHandler;

        public W3CPredicateObjectMapProcessor(IRDFTermGenerator termGenerator, IRdfHandler rdfHandler)
        {
            _termGenerator = termGenerator;
            _rdfHandler = rdfHandler;
        }

        #region Implementation of IPredicateObjectMapProcessor

        public void ProcessPredicateObjectMap(INode subject, IPredicateObjectMap predicateObjectMap, IEnumerable<IUriNode> subjectGraphs, IDataRecord logicalRow)
        {
            var predicates = (from predicateMap in predicateObjectMap.PredicateMaps
                              select _termGenerator.GenerateTerm<IUriNode>(predicateMap, logicalRow)).ToArray();
            var objects = (from objectMap in predicateObjectMap.ObjectMaps
                           select _termGenerator.GenerateTerm<INode>(objectMap, logicalRow)).ToArray();
            var graphs = (from graphMap in predicateObjectMap.GraphMaps
                           select _termGenerator.GenerateTerm<IUriNode>(graphMap, logicalRow)).ToArray();
            var subjectGraphsLocal = subjectGraphs.ToArray();

            foreach (IUriNode predicate in predicates)
            {
                foreach (INode @object in objects)
                {
                    if (!graphs.Any())
                    {
                        var triple = new Triple(subject, predicate, @object);
                        _rdfHandler.HandleTriple(triple);
                    }
                    else
                    {
                        foreach (IUriNode graph in graphs.Union(subjectGraphsLocal))
                        {
                            var triple = new Triple(subject, predicate, @object, graph.Uri);
                            _rdfHandler.HandleTriple(triple);
                        }
                    }
                }
            }
        }

        #endregion
    }
}