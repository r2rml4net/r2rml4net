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
    public class W3CR2RMLProcessor : IR2RMLProcessor, IDisposable
    {
        private readonly DbConnection _connection;
        private readonly ITriplesMapProcessor _triplesMapProcessor;

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which stores the output triples in an in-memory dataset
        /// and uses default RDF term generation and map processing algorithms
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        public W3CR2RMLProcessor(DbConnection connection)
            : this(connection, new RDFTermGenerator())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which adds the generated triples to the supplied dataset
        /// and uses default RDF term generation and map processing algorithms
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="tripleStore">output dataset</param>
        public W3CR2RMLProcessor(DbConnection connection, ITripleStore tripleStore)
            : this(connection, new StoreHandler(tripleStore), new RDFTermGenerator())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which handles the generated triples to the supplied <see cref="IRdfHandler"/>
        /// and uses default RDF term generation and map processing algorithms
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="storeWriter">handler for generated triples</param>
        public W3CR2RMLProcessor(DbConnection connection, IRdfHandler storeWriter)
            : this(connection, storeWriter, new RDFTermGenerator())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which handles the generated triples to the supplied <see cref="IRdfHandler"/>
        /// and uses default map processing algorithm
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        public W3CR2RMLProcessor(DbConnection connection, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new TripleStore(), rdfTermGenerator)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which adds the generated triples to the supplied dataset
        /// and uses default map processing algorithm
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="tripleStore">output dataset</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        public W3CR2RMLProcessor(DbConnection connection, ITripleStore tripleStore, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new StoreHandler(tripleStore), rdfTermGenerator)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which handles the generated triples to the supplied <see cref="IRdfHandler"/>
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="storeWriter">handler for generated triples</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        public W3CR2RMLProcessor(DbConnection connection, IRdfHandler storeWriter, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator, storeWriter))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which processes triples maps with the supplied implementatioon of <see cref="ITriplesMapProcessor"/>
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="triplesMapProcessor"></param>
        protected internal W3CR2RMLProcessor(DbConnection connection, ITriplesMapProcessor triplesMapProcessor)
        {
            _triplesMapProcessor = triplesMapProcessor;
            _connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
        } 

        #endregion

        #region Implementation of IR2RMLProcessor

        /// <summary>
        /// Processes the R2RML mappings and generates RDF triples from source (relational) data
        /// </summary>
        /// <param name="r2RML">R2RML mappings</param>
        public void GenerateTriples(IR2RML r2RML)
        {
            foreach (var triplesMap in r2RML.TriplesMaps)
            {
                _triplesMapProcessor.ProcessTriplesMap(triplesMap, _connection);
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Disposes of the connection
        /// </summary>
        public void Dispose()
        {
            if(_connection.State == ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
        }

        #endregion
    }
}