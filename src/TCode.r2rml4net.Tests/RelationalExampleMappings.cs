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
                var studentsTable = new TableMetadata
                            {
                                new ColumnMetadata
                                {
                                    Name = "Name",
                                    Type = DbType.AnsiString
                                }
                            };
                studentsTable.Name = "Student";

                return new TableCollection
                        {
                            studentsTable
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
                var studentsTable = new TableMetadata
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
                            };
                studentsTable.Name = "Student";

                return new TableCollection
                       {
                           studentsTable
                       };
            }
        }

        /// <summary>
        /// http://www.w3.org/2001/sw/rdb2rdf/test-cases/#D003-1table3columns1row
        /// </summary>
        public static TableCollection D003_1table3columns
        {
            get
            {
                var studentsTable = new TableMetadata
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
                                },
                                new ColumnMetadata
                                {
                                    Name = "LastName",
                                    Type = DbType.AnsiString
                                }
                            };
                studentsTable.Name = "Student";

                return new TableCollection
                       {
                           studentsTable
                       };
            }
        }
    }
}
