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
        public static TableCollection TypedColumns
        {
            get
            {
                var studentsTable = new TableMetadata
                            {
                                new ColumnMetadata
                                {
                                    Name = "Int32",
                                    Type = DbType.Int32
                                },
                                new ColumnMetadata
                                {
                                    Name = "Varchar",
                                    Type = DbType.AnsiString
                                },
                                new ColumnMetadata
                                {
                                    Name = "NVarchar",
                                    Type = DbType.String
                                },
                                new ColumnMetadata
                                {
                                    Name = "Float",
                                    Type = DbType.Single
                                },
                                new ColumnMetadata
                                {
                                    Name = "Double",
                                    Type = DbType.Double
                                },
                                new ColumnMetadata
                                {
                                    Name = "Decimal",
                                    Type = DbType.Decimal
                                },
                                new ColumnMetadata
                                {
                                    Name = "Money",
                                    Type = DbType.Currency
                                },
                                new ColumnMetadata
                                {
                                    Name = "Date",
                                    Type = DbType.Date
                                },
                                new ColumnMetadata
                                {
                                    Name = "Time",
                                    Type = DbType.Time
                                },
                                new ColumnMetadata
                                {
                                    Name = "DateTime",
                                    Type = DbType.DateTime
                                },
                                new ColumnMetadata
                                {
                                    Name = "DateTime2",
                                    Type = DbType.DateTime2
                                },
                                new ColumnMetadata
                                {
                                    Name = "Boolean",
                                    Type = DbType.Boolean
                                },
                                new ColumnMetadata
                                {
                                    Name = "Binary",
                                    Type = DbType.Binary
                                },
                                new ColumnMetadata
                                {
                                    Name = "Int16",
                                    Type = DbType.Int16
                                },
                                new ColumnMetadata
                                {
                                    Name = "Int64",
                                    Type = DbType.Int64
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

        public static TableCollection D006_1table1primarykey1column
        {
            get
            {
                var studentsTable = new TableMetadata
                                        {
                                            new ColumnMetadata
                                                {
                                                    Name = "Name",
                                                    Type = DbType.AnsiString,
                                                    IsPrimaryKey = true
                                                }
                                        };
                studentsTable.Name = "Student";

                return new TableCollection
                       {
                           studentsTable
                       };
            }
        }

        public static TableCollection D008_1table1compositeprimarykey3columns
        {
            get
            {
                var studentsTable = new TableMetadata
                                        {
                                            new ColumnMetadata
                                                {
                                                    Name = "Name",
                                                    Type = DbType.AnsiString,
                                                    IsPrimaryKey = true
                                                },
                                            new ColumnMetadata
                                                {
                                                    Name = "LastName",
                                                    Type = DbType.AnsiString,
                                                    IsPrimaryKey = true
                                                },
                                            new ColumnMetadata
                                                {
                                                    Name = "Age",
                                                    Type = DbType.Int32
                                                }
                                        };
                studentsTable.Name = "Student";

                return new TableCollection
                       {
                           studentsTable
                       };
            }
        }

        public static TableCollection D009_2tables1primarykey1foreignkey
        {
            get
            {
                var studentsTable = new TableMetadata
                                        {
                                            new ColumnMetadata
                                                {
                                                    Name = "ID",
                                                    Type = DbType.Int32,
                                                    IsPrimaryKey = true
                                                },
                                            new ColumnMetadata
                                                {
                                                    Name = "Name",
                                                    Type = DbType.AnsiString
                                                },
                                            new ColumnMetadata
                                                {
                                                    Name = "Sport",
                                                    Type = DbType.Int32
                                                }
                                        };
                studentsTable.Name = "Student";
                studentsTable.ForeignKeys = new[]
                                                {
                                                    new ForeignKeyMetadata
                                                        {
                                                            TableName = "Student",
                                                            ReferencedTableName = "Sport",
                                                            ForeignKeyColumns = new[] {"Sport"},
                                                            ReferencedColumns = new[] {"ID"}
                                                        }
                                                };

                var sportTable = new TableMetadata
                                     {
                                         new ColumnMetadata
                                             {
                                                 Name = "ID",
                                                 Type = DbType.Int32,
                                                 IsPrimaryKey = true
                                             },
                                         new ColumnMetadata
                                             {
                                                 Name = "Name",
                                                 Type = DbType.AnsiString
                                             }
                                     };
                sportTable.Name = "Sport";

                return new TableCollection
                       {
                           studentsTable,
                           sportTable
                       };
            }
        }

        public static TableCollection D010_1table1primarykey3colums
        {
            get
            {
                TableMetadata countryTable = new TableMetadata
                                                 {
                                                     new ColumnMetadata
                                                         {
                                                             Name = "Country Code",
                                                             Type = DbType.Int32,
                                                             IsPrimaryKey = true
                                                         },
                                                     new ColumnMetadata
                                                         {
                                                             Name = "Name",
                                                             Type = DbType.AnsiString
                                                         },
                                                     new ColumnMetadata
                                                         {
                                                             Name = "ISO 3166",
                                                             Type = DbType.AnsiString
                                                         }
                                                 };
                countryTable.Name = "Country Info";

                return new TableCollection
                           {
                               countryTable
                           };
            }
        }

        public static TableCollection D011_M2MRelations
        {
            get
            {
                TableMetadata studentTable = new TableMetadata
                                                 {
                                                     new ColumnMetadata
                                                         {
                                                             Name = "ID",
                                                             Type = DbType.Int32,
                                                             IsPrimaryKey = true
                                                         },
                                                     new ColumnMetadata
                                                         {
                                                             Name = "FirstName",
                                                             Type = DbType.AnsiString
                                                         },
                                                     new ColumnMetadata
                                                         {
                                                             Name = "LastName",
                                                             Type = DbType.AnsiString
                                                         }
                                                 };
                TableMetadata sportTable = new TableMetadata
                                               {
                                                   new ColumnMetadata
                                                       {
                                                           Name = "ID",
                                                           Type = DbType.Int32,
                                                           IsPrimaryKey = true
                                                       },
                                                   new ColumnMetadata
                                                       {
                                                           Name = "Description",
                                                           Type = DbType.AnsiString
                                                       }
                                               };
                TableMetadata relationTable = new TableMetadata
                                                  {
                                                      new ColumnMetadata
                                                          {
                                                              Name = "ID_Student",
                                                             Type = DbType.Int32,
                                                              IsPrimaryKey = true
                                                          },
                                                      new ColumnMetadata
                                                          {
                                                              Name = "ID_Sport",
                                                             Type = DbType.Int32,
                                                              IsPrimaryKey = true
                                                          }
                                                  };

                studentTable.Name = "Student";
                sportTable.Name = "Sport";
                relationTable.Name = "Student_Sport";

                relationTable.ForeignKeys = new[]
                                                {
                                                    new ForeignKeyMetadata
                                                        {
                                                            ForeignKeyColumns = new[] {"ID_Student"},
                                                            ReferencedColumns = new[] {"ID"},
                                                            ReferencedTableName = "Student",
                                                            TableName = "Student_Sport"
                                                        },
                                                    new ForeignKeyMetadata
                                                        {
                                                            ForeignKeyColumns = new[] {"ID_Sport"},
                                                            ReferencedColumns = new[] {"ID"},
                                                            ReferencedTableName = "Sport",
                                                            TableName = "Student_Sport"
                                                        }
                                                };

                return new TableCollection
                           {
                               studentTable,
                               sportTable,
                               relationTable
                           };
            }
        }

        public static TableCollection D014_3tables1primarykey1foreignkey
        {
            get
            {
                TableMetadata empTable = new TableMetadata
                                             {
                                                 new ColumnMetadata
                                                     {
                                                         Name ="empno",
                                                         IsPrimaryKey = true,
                                                         Type = DbType.Int32
                                                     },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "deptno",
                                                          Type = DbType.Int32
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "ename",
                                                          Type = DbType.AnsiString
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "job",
                                                          Type = DbType.AnsiString
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "etype",
                                                          Type = DbType.AnsiString
                                                      }
                                             };
                TableMetadata likesTable = new TableMetadata
                                              {
                                                  new ColumnMetadata
                                                      {
                                                          Name = "id",
                                                          Type = DbType.Int32
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "likeType",
                                                          Type = DbType.AnsiString
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "likedObj",
                                                          Type = DbType.AnsiString
                                                      }
                                              };
                TableMetadata deptTable = new TableMetadata
                                              {
                                                  new ColumnMetadata
                                                      {
                                                          Name = "deptno",
                                                          Type = DbType.Int32
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "dname",
                                                          Type = DbType.AnsiString
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "loc",
                                                          Type = DbType.AnsiString
                                                      }
                                              };

                empTable.Name = "EMP";
                likesTable.Name = "LIKES";
                deptTable.Name = "DEPT";

                empTable.ForeignKeys = new[]
                                           {
                                               new ForeignKeyMetadata
                                                   {
                                                       ForeignKeyColumns = new[] {"deptno"},
                                                       ReferencedColumns = new[] {"deptno"},
                                                       ReferencedTableName = "DEPT",
                                                       TableName = "EMP",
                                                       IsCandidateKeyReference = true
                                                   }
                                           };

                return new TableCollection
                           {
                               empTable,
                               likesTable,
                               deptTable
                           };
            }
        }

        public static TableCollection D015_1table3columns1composityeprimarykey
        {
            get
            {
                TableMetadata countryTable = new TableMetadata
                                             {
                                                 new ColumnMetadata
                                                     {
                                                         Name ="Code",
                                                         IsPrimaryKey = true,
                                                         Type = DbType.AnsiString
                                                     },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "Lan",
                                                          IsPrimaryKey = true,
                                                          Type = DbType.AnsiString
                                                      },
                                                  new ColumnMetadata
                                                      {
                                                          Name = "Name",
                                                          Type = DbType.AnsiString
                                                      }
                                             };

                countryTable.Name = "Country";

                return new TableCollection
                           {
                               countryTable
                           };
            }
        }

        public static TableCollection D016_1table1primarykey10columnsSQLdatatypes
        {
            get
            {
                TableMetadata patientTable = new TableMetadata
                                             {
                                                 new ColumnMetadata
                                                 {
                                                     Name = "ID",
                                                     IsPrimaryKey = true,
                                                     Type = DbType.Int32
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "FirstName",
                                                     Type = DbType.AnsiString
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "LastName",
                                                     Type = DbType.AnsiString
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "Sex",
                                                     Type = DbType.AnsiString
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "Weight",
                                                     Type = DbType.Double
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "Height",
                                                     Type = DbType.Double
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "BirthDate",
                                                     Type = DbType.Date
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "EntranceDate",
                                                     Type = DbType.DateTime
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "PaidInAdvance",
                                                     Type = DbType.Boolean
                                                 },
                                                 new ColumnMetadata
                                                 {
                                                     Name = "Photo",
                                                     Type = DbType.Binary
                                                 }
                                             };

                patientTable.Name = "Patient";

                return new TableCollection
                {
                    patientTable
                };
            }
        }

        public static TableCollection D017_I18NnoSpecialChars
        {
            get
            {
                TableMetadata i18N1 = new TableMetadata
                {
                    new ColumnMetadata
                    {
                        Name="植物名",
                        Type = DbType.String
                    },
                    new ColumnMetadata
                    {
                        Name="使用部",
                        Type = DbType.String
                    },
                    new ColumnMetadata
                    {
                        Name="皿",
                        Type = DbType.String
                    }
                };

                TableMetadata i18N2 = new TableMetadata
                {
                    new ColumnMetadata
                    {
                        Name="名",
                        IsPrimaryKey = true,
                        Type = DbType.String
                    },
                    new ColumnMetadata
                    {
                        Name="使用部",
                        IsPrimaryKey = true,
                        Type = DbType.String
                    },
                    new ColumnMetadata
                    {
                        Name="条件",
                        Type = DbType.String
                    }
                };

                i18N1.ForeignKeys = new[]
                {
                    new ForeignKeyMetadata
                    {
                        ForeignKeyColumns=new[] {"植物名", "使用部"},
                        ReferencedColumns = new [] {"名", "使用部"},
                        ReferencedTableName = "植物",
                        TableName = "成分"
                    }
                };

                i18N1.Name = "成分";
                i18N2.Name = "植物";

                return new TableCollection
                {
                    i18N1, i18N2
                };
            }
        }
    }
}
