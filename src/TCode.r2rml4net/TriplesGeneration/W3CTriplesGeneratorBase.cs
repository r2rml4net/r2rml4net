using System;
using System.Data;
using System.Data.Common;
using TCode.r2rml4net.Mapping;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Base implementation of the triple generation algorithm suggested by R2RML specification. 
    /// It should generate triples for all rows in all triples maps from the input R2RML mappings
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#generated-rdf</remarks>
    public abstract class W3CTriplesGeneratorBase : ITriplesGenerator, IDisposable
    {
        private readonly ITripleStore _tripleStore;
        private readonly DbConnection _connection;

        public ITriplesMapProcessor TriplesMapProcessor { get; private set; }

        protected W3CTriplesGeneratorBase(DbConnection connection)
            : this(connection, new W3CTriplesMapProcessor(new StoreHandler(new TripleStore())))
        {
        }

        protected W3CTriplesGeneratorBase(DbConnection connection, ITripleStore tripleStore)
            : this(connection, new W3CTriplesMapProcessor(new StoreHandler(tripleStore)))
        {
            _tripleStore = tripleStore;
        }

        protected W3CTriplesGeneratorBase(DbConnection connection, IRdfHandler storeWriter)
            : this(connection, new W3CTriplesMapProcessor(storeWriter))
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
            foreach (var triplesMap in r2RML.TriplesMaps)
            {
                TriplesMapProcessor.ProcessTriplesMap(triplesMap, _connection);
            }

            return _tripleStore;
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