#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using Moq;
using Xunit;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.SqlQueryBuilder
{
    public class W3CEffectiveSqlBuilderTests
    {
        private readonly W3CSqlQueryBuilder _sqlQueryBuilder;
        private readonly Mock<IRefObjectMap> _refObjectMap;
        private readonly Mock<ITriplesMap> _triplesMap;

        public W3CEffectiveSqlBuilderTests()
        {
            _sqlQueryBuilder = new W3CSqlQueryBuilder(new MappingOptions());
            _refObjectMap = new Mock<IRefObjectMap>(MockBehavior.Strict);
            _triplesMap = new Mock<ITriplesMap>();
        }

        [Fact]
        public void ReturnsSqlQueryAsEffectiveSql()
        {
            // given
            const string sqlQuery = "SELECT \"a\", \"b\" FROM \"c\" as Table";
            _triplesMap.Setup(tm => tm.TableName);
            _triplesMap.Setup(tm => tm.SqlQuery).Returns(sqlQuery);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

            // then
            Assert.Equal(sqlQuery, sql);
        }

        [Fact]
        public void ReturnsCorrectEffectiveSqlForTable()
        {
            // given
            const string tableName = "Student";
            _triplesMap.Setup(tm => tm.TableName).Returns(tableName);
            _triplesMap.Setup(tm => tm.SqlQuery);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

            // then
            Assert.Equal("SELECT * FROM \"Student\"", sql);
        }

        [Fact]
        public void RefObjectMapWithNoJoinConditionsGiven()
        {
            // given
            _refObjectMap.Setup(rom => rom.ChildEffectiveSqlQuery).Returns("SELECT * FROM \"A\"");
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new JoinCondition[0]);

            // when
            string sql = _sqlQueryBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            Assert.Equal("SELECT * FROM (SELECT * FROM \"A\") AS tmp", sql);
        }

        [Fact]
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

        [Fact]
        public void RefObjectMapWithMultipleJoinConditions()
        {
            // given
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new[]
                                                                       {
                                                                           new JoinCondition("colX", "colY"),
                                                                           new JoinCondition("foo", "bar"),
                                                                           new JoinCondition("dlihc", "\"tnerap\"")
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
                Assert.NotEqual(-1, indexOfCurrent);
                lastIndex = indexOfCurrent + seqElement.Length;
            }
        }

        [Fact]
        public void ThrowsIfInvalidSqlViewHasSqlIdentifier()
        {
            _triplesMap.Setup(tm => tm.SqlVersions).Returns(new[]
                {
                    new Uri("http://no-such-identifier.com")
                });

            Assert.Throws<Exceptions.InvalidSqlVersionException>(() =>
                _sqlQueryBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object)
            );
        }
    }
}