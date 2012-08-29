#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

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