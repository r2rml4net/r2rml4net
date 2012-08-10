using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            }
            else
            {
                IDataReader logicalTable;
                if(!FetchLogicalRows(connection, triplesMap, out logicalTable))
                    return;

                using (logicalTable)
                {
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

                            foreach (var refObjectMap in map.RefObjectMaps.Where(refMap => refMap.SubjectMap != null))
                            {
                                IRefObjectMap objectMap = refObjectMap;
                                var fieldCount = logicalTable.FieldCount;
                                refObjectMapProcesses.Add(() => RefObjectMapProcessor.ProcessRefObjectMap(objectMap, triplesMap.SubjectMap, connection, fieldCount, rdfHandler));
                            }
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