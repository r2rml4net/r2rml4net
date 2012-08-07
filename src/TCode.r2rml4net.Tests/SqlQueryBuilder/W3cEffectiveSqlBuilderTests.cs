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
        private W3CEffectiveSqlBuilder _sqlBuilder;
        Mock<IRefObjectMap> _refObjectMap;
        private Mock<ITriplesMap> _triplesMap;

        [SetUp]
        public void Setup()
        {
            _sqlBuilder = new W3CEffectiveSqlBuilder();
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
            string sql = _sqlBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

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
            string sql = _sqlBuilder.GetEffectiveQueryForTriplesMap(_triplesMap.Object);

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
            string sql = _sqlBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            Assert.AreEqual("SELECT * FROM (SELECT * FROM \"A\") AS tmp", sql);
        }

        [Test]
        public void RefObjectMapWithSingleJoinCondition()
        {
            // given
            _refObjectMap.Setup(rom => rom.JoinConditions).Returns(new[] {new JoinCondition("colX", "colY")});
            _refObjectMap.Setup(rom => rom.ChildEffectiveSqlQuery).Returns("SELECT * FROM A");
            _refObjectMap.Setup(rom => rom.ParentEffectiveSqlQuery).Returns("SELECT * FROM B");

            // when
            string sql = _sqlBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

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
            string sql = _sqlBuilder.GetEffectiveQueryForRefObjectMap(_refObjectMap.Object);

            // then
            AssertContainsSequence(sql,
                                   "SELECT * FROM (SELECT * FROM A) AS child,",
                                   "(SELECT * FROM B) AS parent",
                                   "child.\"colX\"=parent.\"colY\"",
                                   "child.\"foo\"=parent.\"bar\"",
                                   "child.\"dlihc\"=parent.\"tnerap\"");
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