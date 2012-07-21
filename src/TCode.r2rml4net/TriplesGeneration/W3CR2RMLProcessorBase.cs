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
    public abstract class W3CR2RMLProcessorBase : IR2RMLProcessor, IDisposable
    {
        private readonly ITripleStore _tripleStore;
        private readonly DbConnection _connection;

        public ITriplesMapProcessor TriplesMapProcessor { get; private set; }

        #region Constructors

        protected W3CR2RMLProcessorBase(DbConnection connection)
            : this(connection, new W3CTriplesMapProcessor(new RDFTermGenerator(), new StoreHandler(new TripleStore())))
        {
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, ITripleStore tripleStore)
            : this(connection, new W3CTriplesMapProcessor(new RDFTermGenerator(), new StoreHandler(tripleStore)))
        {
            _tripleStore = tripleStore;
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, IRdfHandler storeWriter)
            : this(connection, new W3CTriplesMapProcessor(new RDFTermGenerator(), storeWriter))
        {
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator, new StoreHandler(new TripleStore())))
        {
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, ITripleStore tripleStore, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator, new StoreHandler(tripleStore)))
        {
            _tripleStore = tripleStore;
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, IRdfHandler storeWriter, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator, storeWriter))
        {
        }

        protected W3CR2RMLProcessorBase(DbConnection connection, ITriplesMapProcessor triplesMapProcessor)
        {
            TriplesMapProcessor = triplesMapProcessor;
            _connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
        } 

        #endregion

        #region Implementation of IR2RMLProcessor

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