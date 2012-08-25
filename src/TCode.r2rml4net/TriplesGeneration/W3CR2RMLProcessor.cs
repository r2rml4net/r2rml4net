using System;
using System.Data;
using System.Data.Common;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDF;
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
        private readonly IDbConnection _connection;
        private readonly ITriplesMapProcessor _triplesMapProcessor;
        private readonly MappingOptions _mappingOptions;

        public ITriplesGenerationLog Log { get; set; }

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which stores the output triples in an in-memory dataset
        /// and uses default RDF term generation and map processing algorithms and default mapping options
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        public W3CR2RMLProcessor(IDbConnection connection)
            : this(connection, new RDFTermGenerator(), new MappingOptions())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which handles the generated triples to the supplied <see cref="IRdfHandler"/>
        /// and uses default map processing algorithm and default mapping options
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        public W3CR2RMLProcessor(IDbConnection connection, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator), new MappingOptions())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which stores the output triples in an in-memory dataset
        /// and uses default RDF term generation and map processing algorithms and custom mapping options
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="mappingOptions">options for map processing</param>
        public W3CR2RMLProcessor(IDbConnection connection, MappingOptions mappingOptions)
            : this(connection, new RDFTermGenerator(), mappingOptions)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which handles the generated triples to the supplied <see cref="IRdfHandler"/>
        /// and uses default map processing algorithm and custom mapping options
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        /// <param name="mappingOptions">options for map processing</param>
        public W3CR2RMLProcessor(IDbConnection connection, IRDFTermGenerator rdfTermGenerator, MappingOptions mappingOptions)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator), mappingOptions)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="W3CR2RMLProcessor"/> which processes triples maps with the supplied implementatioon of <see cref="ITriplesMapProcessor"/>
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="triplesMapProcessor"></param>
        /// <param name="mappingOptions">options for map processing</param>
        protected internal W3CR2RMLProcessor(IDbConnection connection, ITriplesMapProcessor triplesMapProcessor, MappingOptions mappingOptions)
        {
            Log = NullLog.Instance;

            _triplesMapProcessor = triplesMapProcessor;
            _mappingOptions = mappingOptions;
            _connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        #endregion

        #region Implementation of IR2RMLProcessor

        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and processes them with the given <see cref="IRdfHandler"/>
        /// </summary>
        public void GenerateTriples(IR2RML r2RML, IRdfHandler rdfHandler)
        {
            bool handlingOk = true;
            IRdfHandler blankNodeReplaceHandler = new BlankNodeSubjectReplaceHandler(rdfHandler);

            blankNodeReplaceHandler.StartRdf();

            foreach (var triplesMap in r2RML.TriplesMaps)
            {
                try
                {
                    _triplesMapProcessor.ProcessTriplesMap(triplesMap, _connection, blankNodeReplaceHandler);
                }
                catch (InvalidTermException e)
                {
                    Log.LogInvalidTermMap(e.TermMap, e.Message);
                    handlingOk = false;
                    if (!_mappingOptions.IgnoreDataErrors)
                        break;
                }
                catch (InvalidMapException e)
                {
                    Log.LogInvaldTriplesMap(triplesMap, e.Message);
                    handlingOk = false;
                    if (!_mappingOptions.IgnoreMappingErrors)
                        break;
                }
            }

            blankNodeReplaceHandler.EndRdf(handlingOk);
            Success = handlingOk;
        }

        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and returns the generated dataset
        /// </summary>
        public ITripleStore GenerateTriples(IR2RML r2RML)
        {
            var tripleStore = new TripleStore();
            GenerateTriples(r2RML, tripleStore);
            return tripleStore;
        }

        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and adds the generated triples to the given <see cref="ITripleStore"/>
        /// </summary>
        public void GenerateTriples(IR2RML r2RML, ITripleStore tripleStore)
        {
            IRdfHandler handler = new StoreHandler(tripleStore);
            GenerateTriples(r2RML, handler);
        }

        public bool Success { get; private set; }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Disposes of the connection
        /// </summary>
        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
        }

        #endregion
    }
}