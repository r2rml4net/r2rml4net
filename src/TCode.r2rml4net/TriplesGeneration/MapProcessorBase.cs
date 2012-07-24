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
        private readonly IRdfHandler _rdfHandler;
        private readonly IRDFTermGenerator _termGenerator;

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="rdfHandler">handler for generated triples</param>
        protected MapProcessorBase(IRDFTermGenerator termGenerator, IRdfHandler rdfHandler)
        {
            _rdfHandler = rdfHandler;
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
        protected internal void AddTriplesToDataSet(INode subject, IEnumerable<IUriNode> predicates, IEnumerable<INode> objects, IEnumerable<IUriNode> graphs)
        {
            var objectsLocal = objects.ToList();
            var graphsLocal = graphs.ToArray();

            foreach (IUriNode predicate in predicates)
            {
                foreach (INode @object in objectsLocal)
                {
                    AddTriplesToDataSet(subject, predicate, @object, graphsLocal);
                }
            }
        }

        protected void AddTriplesToDataSet(INode subject, IUriNode predicate, INode @object, IEnumerable<IUriNode> graphs)
        {
            IEnumerable<IUriNode> graphsLocal = graphs.ToList();
            if (!graphsLocal.Any())
                graphsLocal = new[] { CreateUriNode(new Uri(RrDefaultgraph)) };

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

        /// <summary>
        /// Creates a <see cref="IUriNode"/> for the given <see cref="Uri"/>
        /// </summary>
        protected IUriNode CreateUriNode(Uri uri)
        {
            return _rdfHandler.CreateUriNode(uri);
        }

        protected static IDataReader FetchLogicalRows(IDbConnection connection, string effectiveSqlQuery)
        {
            var command = connection.CreateCommand();
            command.CommandText = effectiveSqlQuery;
            command.CommandType = CommandType.Text;
            return command.ExecuteReader();
        }
    }
}