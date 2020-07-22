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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using TCode.r2rml4net.Exceptions;
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

        /// <summary>
        /// Gets the term generator.
        /// </summary>
        protected IRDFTermGenerator TermGenerator
        {
            get { return _termGenerator; }
        }

        /// <summary>
        /// Adds zero or more triples to the output dataset
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-add-triples</remarks>
        protected internal void AddTriplesToDataSet(
            [AllowNull] INode subject,
            IEnumerable<IUriNode> predicates,
            IEnumerable<INode> objects,
            IEnumerable<IUriNode> graphs,
            IRdfHandler rdfHandler)
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

        /// <summary>
        /// Reads a row of data from a relational database
        /// </summary>
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
                    LogTo.Error("Failed to execute query for map {0}: {1}", map.Node, e.Message);
                    throw new InvalidMapException("Error executing query:", map);
                }
            }

            return true;
        }

        /// <summary>
        /// Ensure the data reader contains unique column names
        /// </summary>
        protected internal void AssertNoDuplicateColumnNames(IDataRecord reader)
        {
            var fieldCount = reader.FieldCount;
            var columnNames = new List<string>(fieldCount);
            for (int colIdx = 0; colIdx < fieldCount; colIdx++)
            {
                string name = reader.GetName(colIdx);
                if (columnNames.Contains(name))
                {
                    throw new InvalidMapException("Sql query contains duplicate names");
                }

                columnNames.Add(name);
            }
        }

        /// <summary>
        /// Handle triple by inserting it to all <paramref name="graphs"/> using the <paramref name="rdfHandler"/>
        /// </summary>
        protected void AddTriplesToDataSet([AllowNull] INode subject, [AllowNull] IUriNode predicate, [AllowNull] INode obj, IEnumerable<IUriNode> graphs, IRdfHandler rdfHandler)
        {
            IEnumerable<IUriNode> graphsLocal = graphs.ToList();
            if (!graphsLocal.Any())
            {
                graphsLocal = new[] { rdfHandler.CreateUriNode(new Uri(RrDefaultgraph)) };
            }

            foreach (IUriNode graph in graphsLocal.Where(g => g != null))
            {
                if (new Uri(RrDefaultgraph).Equals(graph.Uri))
                {
                    AddTripleToDataSet(subject, predicate, obj, rdfHandler);
                }
                else
                {
                    AddTripleToDataSet(subject, predicate, obj, graph, rdfHandler);
                }
            }
        }

        private void AddTripleToDataSet([AllowNull] INode subject, [AllowNull] IUriNode predicate, [AllowNull] INode obj, IRdfHandler rdfHandler)
        {
            if (subject == null || predicate == null || obj == null)
            {
                return;
            }

            var triple = new Triple(subject, predicate, obj);
            rdfHandler.HandleTriple(triple);
        }

        private void AddTripleToDataSet([AllowNull] INode subject, [AllowNull] IUriNode predicate, [AllowNull] INode obj, [AllowNull] IUriNode graph, IRdfHandler rdfHandler)
        {
            if (subject == null || predicate == null || obj == null || graph == null)
            {
                return;
            }

            var triple = new Triple(subject, predicate, obj, graph.Uri);
            rdfHandler.HandleTriple(triple);
        }
    }
}