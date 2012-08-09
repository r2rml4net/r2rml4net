using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.SqlQueryBuilder
{
    [TestFixture]
    public class W3CEffectiveSqlBuilderTests
    {
        private W3CSqlQueryBuilder _sqlQueryBuilder;
        Mock<IRefObjectMap> _refObjectMap;
        private Mock<ITriplesMap> _triplesMap;

        [SetUp]
        public void Setup()
        {
            _sqlQueryBuilder = new W3CSqlQueryBuilder();
            _refObjectMap = new Mock<IRefObjectMap>(MockBehavior.Strict);
            _triplesMap = new Mock<ITriplesMap>();
        }

        [Test]
        public void ReturnsSqlQueryAsEffectiveSql()
        {
            // given
            const string sqlQuery = "SELECT \"a\", \"b\" FROM \"c\" as Table";
            _triplesMap.Setup(tm => tm.TableName);
            _triplesMap.Setup(tm => tm.SqlQuery).Returns(sqlQuery);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

            // then
            Assert.AreEqual(sqlQuery, sql);
        }

        [Test]
        public void ReturnsCorrectEffectiveSqlForTable()
        {
            // given
            const string tableName = "Student";
            _triplesMap.Setup(tm => tm.TableName).Returns(tableName);
            _triplesMap.Setup(tm => tm.SqlQuery);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

            // then
            Assert.AreEqual("SELECT * FROM \"Student\"", sql);
        }

        [Test]
        public void RefObjectMapWithNoJoinConditionsGiven()
        {
            // given
            _refObjectMap.Setup(rom => rom.ChildEffectiveSqlQuery).Returns("SELECT * FROM \"A\"");
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new JoinCondition[0]);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            Assert.AreEqual("SELECT * FROM (SELECT * FROM \"A\") AS tmp", sql);
        }

        [Test]
        public void RefObjectMapWithSingleJoinCondition()
        {
            // given
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new[] { new JoinCondition("colX", "colY") });
            _refObjectMap.Setup(rom => rom.ChildEffectiveSqlQuery).Returns("SELECT * FROM A");
            _refObjectMap.Setup(rom => rom.ParentEffectiveSqlQuery).Returns("SELECT * FROM B");

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            AssertContainsSequence(sql,
                                   "SELECT * FROM (SELECT * FROM A) AS child,",
                                   "(SELECT * FROM B) AS parent",
                                   "WHERE child.\"colX\"=parent.\"colY\"");
        }

        [Test]
        public void RefObjectMapWithMultipleJoinConditions()
        {
            // given
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new[]
                                                                       {
                                                                           new JoinCondition("colX", "colY"),
                                                                           new JoinCondition("foo", "bar"),
                                                                           new JoinCondition("dlihc", "tnerap")
                                                                       });
            _refObjectMap.Setup(rom => rom.ChildEffectiveSqlQuery).Returns("SELECT * FROM A");
            _refObjectMap.Setup(rom => rom.ParentEffectiveSqlQuery).Returns("SELECT * FROM B");

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            AssertContainsSequence(sql,
                                   "SELECT * FROM (SELECT * FROM A) AS child,",
                                   "(SELECT * FROM B) AS parent",
                                   "child.\"colX\"=parent.\"colY\"",
                                   "child.\"foo\"=parent.\"bar\"",
                                   "child.\"dlihc\"=parent.\"tnerap\"");
        }

        [Test]
        public void ThrowsIfNoForeignKeyReferencedTableHasPrimaryKey()
        {
            // given
            TableMetadata table = new TableMetadata
                {
                    ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata {ReferencedTableHasPrimaryKey = false},
                            new ForeignKeyMetadata {ReferencedTableHasPrimaryKey = false},
                            new ForeignKeyMetadata {ReferencedTableHasPrimaryKey = false},
                            new ForeignKeyMetadata {ReferencedTableHasPrimaryKey = false}
                        }
                };

            // then
            Assert.Throws<ArgumentException>(() => _sqlQueryBuilder.GetR2RMLViewForJoinedTables(table));
        }

        [Test]
        public void ReturnsJoinedQueryForSingleReferenceSimplePrimaryKey()
        {
            // given
            TableMetadata referenced = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id", IsPrimaryKey = true }
                };
            referenced.Name = "target";
            TableMetadata table = new TableMetadata
            {
                Name = "source",
                ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"unique1", "unique2"},
                                    ForeignKeyColumns = new []{"candidate1", "candidate2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = true,
                                    TableName = "source",
                                    ReferencedTable = referenced
                                }, 
                        }
            };

            // when
            var query = _sqlQueryBuilder.GetR2RMLViewForJoinedTables(table);

            // then
            AssertContainsSequence(query,
                "SELECT child.*,",
                "p1.\"Id\" as \"targetId\"",
                "FROM \"source\" as child",
                "LEFT JOIN \"target\" as p1",
                "ON",
                "p1.\"unique1\" = child.\"candidate1\"",
                "AND",
                "p1.\"unique2\" = child.\"candidate2\"");
        }

        [Test]
        public void ReturnsJoinedQueryForSingleReferenceCompositePrimaryKey()
        {
            // given
            TableMetadata referenced = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id", IsPrimaryKey = true },
                    new ColumnMetadata { Name = "Id2", IsPrimaryKey = true }
                };
            referenced.Name = "target";
            TableMetadata table = new TableMetadata
            {
                Name = "source",
                ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"unique1", "unique2"},
                                    ForeignKeyColumns = new []{"candidate1", "candidate2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = true,
                                    TableName = "source",
                                    ReferencedTable = referenced
                                }, 
                        }
            };

            // when
            var query = _sqlQueryBuilder.GetR2RMLViewForJoinedTables(table);

            // then
            AssertContainsSequence(query,
                "SELECT child.*,",
                "p1.\"Id\" as \"targetId\"",
                "p1.\"Id2\" as \"targetId2\"",
                "FROM \"source\" as child",
                "LEFT JOIN \"target\" as p1",
                "ON",
                "p1.\"unique1\" = child.\"candidate1\"",
                "AND",
                "p1.\"unique2\" = child.\"candidate2\"");
        }

        [Test]
        public void ReturnsJoinedQueryForMultipleReferencesMixedPrimaryKeys()
        {
            // given
            TableMetadata referenced = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id", IsPrimaryKey = true }
                };
            referenced.Name = "target";
            TableMetadata referenced1 = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id", IsPrimaryKey = true },
                    new ColumnMetadata { Name = "Id1", IsPrimaryKey = true }
                };
            referenced1.Name = "second";
            TableMetadata table = new TableMetadata
            {
                Name = "source",
                ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"unique1", "unique2"},
                                    ForeignKeyColumns = new []{"candidate1", "candidate2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = true,
                                    TableName = "source",
                                    ReferencedTable = referenced
                                }, 
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"pk1", "pk2"},
                                    ForeignKeyColumns = new []{"fk1", "fk2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = true,
                                    TableName = "source",
                                    ReferencedTable = referenced1
                                }
                        }
            };

            // when
            var query = _sqlQueryBuilder.GetR2RMLViewForJoinedTables(table);

            // then
            AssertContainsSequence(query,
                "SELECT child.*,",
                "p1.\"Id\" as \"targetId\"",
                "p2.\"Id\" as \"secondId\"",
                "p2.\"Id1\" as \"secondId1\"",
                "FROM \"source\" as child",
                "LEFT JOIN \"target\" as p1",
                "p1.\"unique1\" = child.\"candidate1\"",
                "p1.\"unique2\" = child.\"candidate2\"",
                "LEFT JOIN \"second\" as p2",
                "p2.\"pk1\" = child.\"fk1\"",
                "p2.\"pk2\" = child.\"fk2\"");
        }

        [Test]
        public void ReturnsJoinedQueryOnlyForTablesWith()
        {
            // given
            TableMetadata referenced = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id" }
                };
            referenced.Name = "target";
            TableMetadata referenced1 = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id", IsPrimaryKey = true },
                    new ColumnMetadata { Name = "Id1", IsPrimaryKey = true }
                };
            referenced1.Name = "second";
            TableMetadata table = new TableMetadata
            {
                Name = "source",
                ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"unique1", "unique2"},
                                    ForeignKeyColumns = new []{"candidate1", "candidate2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = false,
                                    TableName = "source",
                                    ReferencedTable = referenced
                                }, 
                            new ForeignKeyMetadata
                                {
                                    ReferencedColumns = new []{"pk1", "pk2"},
                                    ForeignKeyColumns = new []{"fk1", "fk2"},
                                    IsCandidateKeyReference = true,
                                    ReferencedTableHasPrimaryKey = true,
                                    TableName = "source",
                                    ReferencedTable = referenced1
                                }
                        }
            };

            // when
            var query = _sqlQueryBuilder.GetR2RMLViewForJoinedTables(table);

            // then
            AssertContainsSequence(query,
                "SELECT child.*,",
                "p1.\"Id\" as \"secondId\"",
                "p1.\"Id1\" as \"secondId1\"",
                "FROM \"source\" as child",
                "LEFT JOIN \"second\" as p1",
                "p1.\"pk1\" = child.\"fk1\"",
                "p1.\"pk2\" = child.\"fk2\"");
        }

        static void AssertContainsSequence(string actualString, params string[] expectedValues)
        {
            int lastIndex = 0;
            foreach (var seqElement in expectedValues)
            {
                int indexOfCurrent = actualString.IndexOf(seqElement, lastIndex, StringComparison.Ordinal);
                Assert.AreNotEqual(-1, indexOfCurrent, string.Format("Sequence element\r\n\r\n{0}\r\n\r\nnot found in\r\n\r\n{1}", seqElement, actualString));
                lastIndex = indexOfCurrent + seqElement.Length;
            }
        }
    }
}