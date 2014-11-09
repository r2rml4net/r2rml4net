#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    internal class W3CTriplesMapProcessor : MapProcessorBase, ITriplesMapProcessor
    {
        public IPredicateObjectMapProcessor PredicateObjectMapProcessor { get; set; }
        public IRefObjectMapProcessor RefObjectMapProcessor { get; set; }

        public W3CTriplesMapProcessor(IRDFTermGenerator termGenerator)
            : this(
                termGenerator,
                new W3CPredicateObjectMapProcessor(termGenerator),
                new W3CRefObjectMapProcessor(termGenerator))
        {
        }

        public W3CTriplesMapProcessor(
            IRDFTermGenerator termGenerator,
            IPredicateObjectMapProcessor predicateObjectMapProcessor,
            IRefObjectMapProcessor refObjectMapProcessor)
            : base(termGenerator)
        {
            PredicateObjectMapProcessor = predicateObjectMapProcessor;
            RefObjectMapProcessor = refObjectMapProcessor;
            Log = NullLog.Instance;
        }

        #region Implementation of ITriplesMapProcessor

        public void ProcessTriplesMap(ITriplesMap triplesMap, IDbConnection connection, IRdfHandler rdfHandler)
        {
            IList<Action> refObjectMapProcesses = new List<Action>();

            if (triplesMap.SubjectMap == null)
            {
                Log.LogMissingSubject(triplesMap);
                throw new InvalidMapException("Subject is missing", triplesMap);
            }
            else
            {
                IDataReader logicalTable;
                if(!FetchLogicalRows(connection, triplesMap, out logicalTable))
                    return;

                using (logicalTable)
                {
                    AssertNoDuplicateColumnNames(logicalTable);

                    IEnumerable<Uri> classes = triplesMap.SubjectMap.Classes;
                    while (logicalTable.Read())
                    {
                        var subject = TermGenerator.GenerateTerm<INode>(triplesMap.SubjectMap, logicalTable);
                        var graphs = (from graph in triplesMap.SubjectMap.GraphMaps
                                      select TermGenerator.GenerateTerm<IUriNode>(graph, logicalTable)).ToArray();

                        AddTriplesToDataSet(
                            subject,
                            new[] { rdfHandler.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type")) },
                            classes.Select(rdfHandler.CreateUriNode).Cast<INode>().ToList(),
                            graphs,
                            rdfHandler
                            );

                        foreach (IPredicateObjectMap map in triplesMap.PredicateObjectMaps)
                        {
                            PredicateObjectMapProcessor.ProcessPredicateObjectMap(subject, map, graphs, logicalTable, rdfHandler);
                        }
                    }

                    foreach (IPredicateObjectMap map in triplesMap.PredicateObjectMaps)
                    {
                        foreach (var refObjectMap in map.RefObjectMaps.Where(refMap => refMap.SubjectMap != null))
                        {
                            IRefObjectMap objectMap = refObjectMap;
                            var fieldCount = logicalTable.FieldCount;
                            refObjectMapProcesses.Add(() => RefObjectMapProcessor.ProcessRefObjectMap(objectMap, triplesMap.SubjectMap, connection, fieldCount, rdfHandler));
                        }
                    }
                }

                foreach (var refObjectMapProcess in refObjectMapProcesses)
                {
                    refObjectMapProcess.Invoke();
                }
            }
        }

        #endregion
    }
}