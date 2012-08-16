using System.Data;
using System.Linq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Default implementation of <see cref="IRefObjectMapProcessor"/> generating triples for joined triples maps
    /// </summary>
    /// <remarks>see http://www.w3.org/TR/r2rml/#generated-triples</remarks>
    class W3CRefObjectMapProcessor : MapProcessorBase, IRefObjectMapProcessor
    {
        public W3CRefObjectMapProcessor(IRDFTermGenerator termGenerator)
            : base(termGenerator)
        {
        }

        #region Implementation of IRefObjectMapProcessor

        public void ProcessRefObjectMap(IRefObjectMap refObjectMap, ISubjectMap subjectMap, IDbConnection dbConnection, int childColumnsCount, IRdfHandler rdfHandler)
        {
            IDataReader dataReader;
            if (!FetchLogicalRows(dbConnection, refObjectMap, out dataReader))
                return;

            using (dataReader)
            {
                while (dataReader.Read())
                {
                    var childRow = WrapDataRecord(dataReader, childColumnsCount,
                                                  ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns);
                    IDataRecord parentRow;
                    if (childColumnsCount == dataReader.FieldCount)
                        parentRow = childRow;
                    else
                        parentRow = WrapDataRecord(dataReader, childColumnsCount,
                                                   ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns);

                    AssertNoDuplicateColumnNames(parentRow);
                    AssertNoDuplicateColumnNames(childRow);

                    var subject = TermGenerator.GenerateTerm<INode>(subjectMap, childRow);
                    var predicates = from predicateMap in refObjectMap.PredicateObjectMap.PredicateMaps
                                     select TermGenerator.GenerateTerm<IUriNode>(predicateMap, childRow);
                    var @object = TermGenerator.GenerateTerm<INode>(refObjectMap.SubjectMap, parentRow);
                    var subjectGraphs = (from graphMap in subjectMap.GraphMaps
                                         select TermGenerator.GenerateTerm<IUriNode>(graphMap, childRow)).ToList();
                    var predObjectGraphs = (from graphMap in refObjectMap.PredicateObjectMap.GraphMaps
                                            select TermGenerator.GenerateTerm<IUriNode>(graphMap, childRow)).ToList();

                    foreach (IUriNode predicate in predicates)
                    {
                        AddTriplesToDataSet(subject, predicate, @object, subjectGraphs.Union(predObjectGraphs), rdfHandler);
                    }
                }
            }
        }

        #endregion

        internal IDataRecord WrapDataRecord(IDataRecord dataRecord, int columnLimit, ColumnConstrainedDataRecord.ColumnLimitType limitType)
        {
            return new ColumnConstrainedDataRecord(dataRecord, columnLimit, limitType);
        }
    }
}