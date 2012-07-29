using System;
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
        public ITriplesGenerationLog Log { get; set; }
        public IPredicateObjectMapProcessor PredicateObjectMapProcessor { get; set; }
        public IRefObjectMapProcessor RefObjectMapProcessor { get; set; }

        public W3CTriplesMapProcessor(IRDFTermGenerator termGenerator, IRdfHandler storeWriter)
            : this(
                termGenerator,
                storeWriter,
                new W3CPredicateObjectMapProcessor(termGenerator, storeWriter),
                new W3CRefObjectMapProcessor(termGenerator, storeWriter))
        {
        }

        public W3CTriplesMapProcessor(
            IRDFTermGenerator termGenerator,
            IRdfHandler storeWriter,
            IPredicateObjectMapProcessor predicateObjectMapProcessor,
            IRefObjectMapProcessor refObjectMapProcessor)
            : base(termGenerator, storeWriter)
        {
            PredicateObjectMapProcessor = predicateObjectMapProcessor;
            RefObjectMapProcessor = refObjectMapProcessor;
            Log = NullLog.Instance;
        }

        #region Implementation of ITriplesMapProcessor

        public void ProcessTriplesMap(ITriplesMap triplesMap, IDbConnection connection)
        {
            if (triplesMap.SubjectMap == null)
            {
                Log.LogMissingSubject(triplesMap);
            }
            else
            {
                using (IDataReader logicalTable = FetchLogicalRows(connection, triplesMap.EffectiveSqlQuery))
                {
                    IEnumerable<Uri> classes = triplesMap.SubjectMap.Classes;
                    while (logicalTable.Read())
                    {
                        var subject = TermGenerator.GenerateTerm<INode>(triplesMap.SubjectMap, logicalTable);
                        var graphs = (from graph in triplesMap.SubjectMap.GraphMaps
                                      select TermGenerator.GenerateTerm<IUriNode>(graph, logicalTable)).ToArray();

                        AddTriplesToDataSet(
                            subject,
                            new[] { CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type")) },
                            classes.Select(CreateUriNode).Cast<INode>().ToList(),
                            graphs
                            );

                        foreach (IPredicateObjectMap map in triplesMap.PredicateObjectMaps)
                        {
                            PredicateObjectMapProcessor.ProcessPredicateObjectMap(subject, map, graphs, logicalTable);

                            foreach (
                                IRefObjectMap refObjectMap in
                                    map.RefObjectMaps.Where(refMap => refMap.SubjectMap != null))
                            {
                                RefObjectMapProcessor.ProcessRefObjectMap(refObjectMap, connection,
                                                                          logicalTable.FieldCount);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}