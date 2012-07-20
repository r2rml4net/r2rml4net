using System;
using System.Data;
using System.Data.Common;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Base implementation of the triple generation algorithm suggested by R2RML specification. 
    /// It should generate triples for all rows in all triples maps from the input R2RML mappings
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#generated-rdf</remarks>
    public abstract class W3CTriplesGeneratorBase : ITriplesGenerator, IDisposable
    {
        public ITriplesMapProcessor TriplesMapProcessor { get; private set; }
        private readonly DbConnection _connection;

        protected W3CTriplesGeneratorBase(DbConnection connection)
            : this(connection, new TriplesMapProcessor())
        {
        }

        protected W3CTriplesGeneratorBase(DbConnection connection, ITriplesMapProcessor triplesMapProcessor)
        {
            TriplesMapProcessor = triplesMapProcessor;
            _connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        #region Implementation of ITriplesGenerator

        public ITripleStore GenerateTriples(IR2RML r2RML)
        {
            var store = new TripleStore();

            foreach (var triplesMap in r2RML.TriplesMaps)
            {
                foreach (IGraph graph in TriplesMapProcessor.ProcessTriplesMap(triplesMap, _connection))
                {
                    store.Add(graph, true);
                }
            }

            return store;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if(_connection.State == ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
        }

        #endregion
    }
}