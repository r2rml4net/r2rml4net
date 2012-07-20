using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    internal class TriplesMapProcessor : ITriplesMapProcessor
    {
        private readonly IRDFTermGenerator _termGenerator;
        private readonly ITripleStore _generatedDataset;

        public ITriplesGenerationLog Log { get; set; }

        public TriplesMapProcessor(IRDFTermGenerator termGenerator)
        {
            Log = NullLog.Instance;
            _generatedDataset = new TripleStore();
            _termGenerator = termGenerator;
        }

        public TriplesMapProcessor()
            : this(new RDFTermGenerator())
        {
        }

        #region Implementation of ITriplesMapProcessor

        public IEnumerable<IGraph> ProcessTriplesMap(ITriplesMap triplesMap, IDbConnection connection)
        {
            if (triplesMap.SubjectMap == null)
            {
                Log.LogMissingSubject(triplesMap);
            }
            else
            {
                IDataReader logicalTable = FetchLogicalRows(connection, triplesMap.EffectiveSqlQuery);
                IEnumerable<Uri> classes = triplesMap.SubjectMap.Classes;
                while(logicalTable.Read())
                {
                    var subject = _termGenerator.GenerateTerm(triplesMap.SubjectMap, logicalTable);
                    var graphs = (from graph in triplesMap.SubjectMap.Graphs
                                 select _termGenerator.GenerateTerm(graph, logicalTable)).ToArray();
                }
            }

            return _generatedDataset.Graphs;
        }

        #endregion

        private static IDataReader FetchLogicalRows(IDbConnection connection, string effectiveSqlQuery)
        {
            var command = connection.CreateCommand();
            command.CommandText = effectiveSqlQuery;
            command.CommandType = CommandType.Text;
            return command.ExecuteReader();
        }
    }
}