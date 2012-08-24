using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
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

        public ITriplesGenerationLog Log { get; set; }

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

        protected internal bool FetchLogicalRows(IDbConnection connection, IQueryMap map, out IDataReader dataReader)
        {
            dataReader = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = map.EffectiveSqlQuery;
                command.CommandType = CommandType.Text;
                try
                {
                    dataReader = command.ExecuteReader();
                }
                catch (Exception e)
                {
                    LogSqlExecuteError(map, e.Message);
                    throw new InvalidTriplesMapException("Error executing query");
                }
            }

            return true;
        }

        protected internal void AssertNoDuplicateColumnNames(IDataRecord reader)
        {
            var fieldCount = reader.FieldCount;
            var columnNames = new List<string>(fieldCount);
            for (int colIdx = 0; colIdx < fieldCount; colIdx++)
            {
                string name = reader.GetName(colIdx);
                if(columnNames.Contains(name))
                {
                    throw new InvalidTriplesMapException("Sql query contains duplicate names");
                }
                columnNames.Add(name);
            }
        }

        private void LogSqlExecuteError(IQueryMap map, string message)
        {
            Log.LogQueryExecutionError(map, message);
        }
    }
}