#region Licence
// Copyright (C) 2012-2020 Tomasz Pluskiewicz
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anotar.NLog;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Storage;
using VDS.RDF.Writing.Formatting;

namespace TCode.r2rml4net.CLI
{
    public class EfficientBatchSparqlInsertHandler : BaseHandler, IRdfHandler
    {
        private readonly ReadWriteSparqlConnector _manager;
        private readonly int _batchSize;
        private readonly IList<Triple> _batch = new List<Triple>();
        private readonly SparqlFormatter _formatter = new SparqlFormatter();

        public EfficientBatchSparqlInsertHandler(ReadWriteSparqlConnector manager, int batchSize)
        {
            _manager = manager;
            _batchSize = batchSize;
        }

        public void StartRdf()
        {
        }

        public void EndRdf(bool ok)
        {
            if (_batch.Any())
            {
                InsertBatch();
            }
        }

        public bool HandleNamespace(string prefix, Uri namespaceUri)
        {
            return true;
        }

        public bool HandleBaseUri(Uri baseUri)
        {
            return true;
        }

        public bool HandleTriple(Triple t)
        {
            this._batch.Add(t);
            if (_batch.Count >= this._batchSize)
            {
                return this.InsertBatch();
            }

            return true;
        }

        public bool AcceptsAll { get; } = true;

        private bool InsertBatch()
        {
            try
            {
                LogTo.Debug("Saving batch of quads");
                var insert = new StringBuilder();
                insert.AppendLine("INSERT DATA {");

                var graphs = from triple in this._batch
                    group triple by triple.GraphUri
                    into g
                    select g;

                foreach (var graph in graphs)
                {
                    if (graph.Key != null)
                    {
                        insert.AppendLine($"GRAPH <{_formatter.FormatUri(graph.Key)}> {{");
                    }

                    foreach (var triple in graph)
                    {
                        insert.AppendLine(_formatter.Format(triple));
                    }

                    if (graph.Key != null)
                    {
                        insert.AppendLine("}");
                    }
                }

                insert.AppendLine("}");

                this._manager.Update(insert.ToString());

                this._batch.Clear();
            }
            catch (Exception ex)
            {
                LogTo.Error("Failed saving quads to SPARQL Endpoint: {0}", ex.Message);
                return false;
            }

            return true;
        }
    }
}