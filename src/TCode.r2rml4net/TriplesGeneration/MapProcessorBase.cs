using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Convenience base class for classes processing R2RML maps
    /// </summary>
    public abstract class MapProcessorBase
    {
        /// <summary>
        /// The rr:defaultGraph URI string
        /// </summary>
        protected const string RrDefaultgraph = "http://www.w3.org/ns/r2rml#defaultGraph";
        private readonly IRDFTermGenerator _termGenerator;

        /// <summary>
        /// Creates an instance
        /// </summary>
        protected MapProcessorBase(IRDFTermGenerator termGenerator)
        {
            _termGenerator = termGenerator;
        }

        protected IRDFTermGenerator TermGenerator
        {
            get { return _termGenerator; }
        }

        /// <summary>
        /// Adds zero or more triples to the output dataset
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-add-triples</remarks>
        protected internal void AddTriplesToDataSet(INode subject, IEnumerable<IUriNode> predicates, IEnumerable<INode> objects, IEnumerable<IUriNode> graphs, IRdfHandler rdfHandler)
        {
            var objectsLocal = objects.ToList();
            var graphsLocal = graphs.ToArray();

            foreach (IUriNode predicate in predicates)
            {
                foreach (INode @object in objectsLocal)
                {
                    AddTriplesToDataSet(subject, predicate, @object, graphsLocal, rdfHandler);
                }
            }
        }

        protected void AddTriplesToDataSet(INode subject, IUriNode predicate, INode @object, IEnumerable<IUriNode> graphs, IRdfHandler rdfHandler)
        {
            IEnumerable<IUriNode> graphsLocal = graphs.ToList();
            if (!graphsLocal.Any())
                graphsLocal = new[] { rdfHandler.CreateUriNode(new Uri(RrDefaultgraph)) };

            foreach (IUriNode graph in graphsLocal.Where(g => g != null))
            {
                if (new Uri(RrDefaultgraph).Equals(graph.Uri))
                {
                    AddTripleToDataSet(subject, predicate, @object, rdfHandler);
                }
                else
                {
                    AddTripleToDataSet(subject, predicate, @object, graph, rdfHandler);
                }
            }
        }

        private void AddTripleToDataSet(INode subject, IUriNode predicate, INode @object, IRdfHandler rdfHandler)
        {
            if (subject == null || predicate == null || @object == null)
                return;

            var triple = new Triple(subject, predicate, @object);
            rdfHandler.HandleTriple(triple);
        }

        private void AddTripleToDataSet(INode subject, IUriNode predicate, INode @object, IUriNode graph, IRdfHandler rdfHandler)
        {
            if (subject == null || predicate == null || @object == null || graph == null)
                return;

            var triple = new Triple(subject, predicate, @object, graph.Uri);
            rdfHandler.HandleTriple(triple);
        }

        protected static IDataReader FetchLogicalRows(IDbConnection connection, string effectiveSqlQuery)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = effectiveSqlQuery;
                command.CommandType = CommandType.Text;
                return command.ExecuteReader();
            }
        }
    }
}