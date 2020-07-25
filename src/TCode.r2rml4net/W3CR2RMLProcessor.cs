#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
//
// ------------------------------------------------------------------------
//
// This file is part of r2rml4net.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.
//
// ------------------------------------------------------------------------
//
// r2rml4net may alternatively be used under the LGPL licence
//
// http://www.gnu.org/licenses/lgpl.html
//
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using System.Data;
using Anotar.NLog;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.RDF;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;

namespace TCode.r2rml4net
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
        private readonly StoreCountHandler _counter = new StoreCountHandler();
        private readonly MappingOptions _options;

        /// <summary>
        /// Initializes an instance of <see cref="W3CR2RMLProcessor"/> which generates triples using the default <see cref="RDFTermGenerator"/>
        /// and uses default RDF term generation and map processing algorithms
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="options">options to influence mapping process</param>
        public W3CR2RMLProcessor(IDbConnection connection, MappingOptions options)
            : this(connection, new RDFTermGenerator(options))
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="W3CR2RMLProcessor"/> which generates triples using the supplied <see cref="IRDFTermGenerator"/>
        /// and uses default map processing algorithm
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        /// <param name="rdfTermGenerator">generator of <see cref="INode"/>s for subject maps, predicate maps, object maps and graph maps</param>
        public W3CR2RMLProcessor(IDbConnection connection, IRDFTermGenerator rdfTermGenerator)
            : this(connection, new W3CTriplesMapProcessor(rdfTermGenerator))
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="W3CR2RMLProcessor"/> which processes triples maps with the supplied implementatioon of <see cref="ITriplesMapProcessor"/>
        /// </summary>
        /// <param name="connection">connection to datasource</param>
        protected internal W3CR2RMLProcessor(IDbConnection connection, ITriplesMapProcessor triplesMapProcessor)
        {
            _triplesMapProcessor = triplesMapProcessor;
            _connection = connection;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Gets a value indicating whether generating triples had no errors
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets a value indicating whether data errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreDataErrors { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether mapping errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreMappingErrors { get; set; } = true;

        public int TriplesGenerated => this._counter.TripleCount;

        public int GraphsGenerated => this._counter.GraphCount;

        /// <summary>
        /// Generates triples from <paramref name="mappings"/> mappings and processes them with the given <see cref="IRdfHandler"/>
        /// </summary>
        public void GenerateTriples(IR2RML mappings, IRdfHandler rdfHandler)
        {
            bool handlingOk = true;
            IRdfHandler blankNodeReplaceHandler = new BlankNodeSubjectReplaceHandler(rdfHandler);
            IRdfHandler combinedHandler = new MultiHandler(new []
            {
                this._counter,
                blankNodeReplaceHandler
            });

            combinedHandler.StartRdf();

            foreach (var triplesMap in mappings.TriplesMaps)
            {
                try
                {
                    LogTo.Info("Processing triples map {0}", triplesMap.Node);
                    _triplesMapProcessor.ProcessTriplesMap(triplesMap, _connection, combinedHandler);
                }
                catch (InvalidTermException e)
                {
                    LogTo.Error("Term map {0} was invalid: {1}", e.TermMap.Node, e.Message);
                    handlingOk = false;
                    if (!this.IgnoreDataErrors)
                    {
                        break;
                    }
                }
                catch (InvalidMapException e)
                {
                    LogTo.Error("Triples map {0} was invalid: {1}", triplesMap.Node, e.Message);
                    handlingOk = false;
                    if (!this.IgnoreMappingErrors)
                    {
                        break;
                    }
                }
            }

            combinedHandler.EndRdf(handlingOk);
            Success = handlingOk;
        }

        /// <summary>
        /// Generates triples from <paramref name="mappings"/> mappings and returns the generated dataset
        /// </summary>
        public ITripleStore GenerateTriples(IR2RML mappings)
        {
            var tripleStore = new TripleStore();
            GenerateTriples(mappings, tripleStore);
            return tripleStore;
        }

        /// <summary>
        /// Generates triples from <paramref name="mappings"/> mappings and adds the generated triples to the given <see cref="ITripleStore"/>
        /// </summary>
        public void GenerateTriples(IR2RML mappings, ITripleStore tripleStore)
        {
            IRdfHandler handler = new StoreHandler(tripleStore);
            GenerateTriples(mappings, handler);
        }

        /// <summary>
        /// Disposes of the connection
        /// </summary>
        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            _connection.Dispose();
        }
    }
}