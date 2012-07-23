using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    internal abstract class MapProcessorBase
    {
        protected const string RrDefaultgraph = "http://www.w3.org/ns/r2rml#defaultGraph";
        private readonly IRdfHandler _rdfHandler;

        protected MapProcessorBase(IRdfHandler rdfHandler)
        {
            _rdfHandler = rdfHandler;
        }

        protected void AddTriplesToDataSet(INode subject, IEnumerable<IUriNode> predicates, IList<INode> objects, IList<IUriNode> graphs)
        {
            var graphsLocal = graphs;
            if (!graphsLocal.Any())
                graphsLocal = new[] {CreateUriNode(new Uri(RrDefaultgraph))};

            foreach (IUriNode predicate in predicates)
            {
                foreach (INode @object in objects)
                {
                    foreach (IUriNode graph in graphsLocal)
                    {
                        if (new Uri(RrDefaultgraph).Equals(graph.Uri))
                        {
                            var triple = new Triple(subject, predicate, @object);
                            _rdfHandler.HandleTriple(triple);
                        }
                        else
                        {
                            var triple = new Triple(subject, predicate, @object, graph.Uri);
                            _rdfHandler.HandleTriple(triple);
                        }
                    }
                }
            }
        }

        protected IUriNode CreateUriNode(Uri uri)
        {
            return _rdfHandler.CreateUriNode(uri);
        }
    }
}