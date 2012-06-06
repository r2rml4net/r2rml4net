using System.Data;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests
{
    public class RelationalTestMappings
    {
        /// <summary>
        /// http://www.w3.org/2001/sw/rdb2rdf/test-cases/#D001-1table1column1row
        /// </summary>
        public static TableCollection D001_1table1column
        {
            get
            {
                return new TableCollection
                        {
                            new TableMetadata
                            {
                                new ColumnMetadata
                                {
                                    Name = "Name",
                                    Type = DbType.AnsiString
                                }
                            }
                        };
            }
        }

        /// <summary>
        /// http://www.w3.org/2001/sw/rdb2rdf/test-cases/#D002-1table2columns1row
        /// </summary>
        public static TableCollection D002_1table2columns
        {
            get
            {
                return new TableCollection
                        {
                            new TableMetadata
                            {
                                new ColumnMetadata
                                {
                                    Name = "ID",
                                    Type = DbType.Int32
                                },
                                new ColumnMetadata
                                {
                                    Name = "Name",
                                    Type = DbType.AnsiString
                                }
                            }
                        };
            }
        }
    }
}
