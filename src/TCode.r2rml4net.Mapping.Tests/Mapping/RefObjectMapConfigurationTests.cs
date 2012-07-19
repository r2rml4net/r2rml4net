using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;
        private Mock<ITriplesMapConfiguration> _referencedTriplesMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            predicateObjectMap.Setup(map => map.Node).Returns(
                graph.CreateUriNode(new Uri("http://test.example.com/PredicateObjectMap")));

            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _parentTriplesMap.Setup(map => map.Node).Returns(graph.CreateUriNode(new Uri("http://test.example.com/TriplesMap")));
            _referencedTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap.Setup(tm => tm.Node).Returns(graph.CreateUriNode(new Uri("http://test.example.com/OtherTriplesMap")));

            _refObjectMap = new RefObjectMapConfiguration(predicateObjectMap.Object, _parentTriplesMap.Object, _referencedTriplesMap.Object, graph);
        }

        [Test]
        public void CreatingAssertsRequiredTriples()
        {
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankObject("http://test.example.com/PredicateObjectMap", UriConstants.RrObjectMapProperty);
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrParentTriplesMapProperty, "http://test.example.com/TriplesMap");
        }

        [Test]
        public void CanCreateJoinConditions()
        {
            // given
            const string childColumn = "child";
            const string parentColumn = "parent";

            // when
            _refObjectMap.AddJoinCondition(childColumn + "1", parentColumn + "1");
            _refObjectMap.AddJoinCondition(childColumn + "2", parentColumn + "2");

            // then
            Assert.AreEqual(2, _refObjectMap.JoinConditions.Count());
            Assert.IsNotNull(_refObjectMap.JoinConditions.SingleOrDefault(jc => jc.ChildColumn == "child1" && jc.ParentColumn == "parent1"));
            Assert.IsNotNull(_refObjectMap.JoinConditions.SingleOrDefault(jc => jc.ChildColumn == "child2" && jc.ParentColumn == "parent2"));
        }

        [Test]
        public void ReturnSimpleEffectiveSqlWhenNoJoinConditionsGiven()
        {
            // given
            _parentTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM A");
            _referencedTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM B");

            // then
            Assert.AreEqual("SELECT * FROM (SELECT * FROM A) AS tmp", _refObjectMap.EffectiveSQLQuery);
        }

        [Test]
        public void ReturnSimpleEffectiveSqlWithSingleJoinCondition()
        {
            // given
            _parentTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM A");
            _referencedTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM B");

            // when
            _refObjectMap.AddJoinCondition("colX", "colY");

            // then
            AssertContainsSequence(_refObjectMap.EffectiveSQLQuery,
                                   "SELECT * FROM (SELECT * FROM A) AS child,",
                                   "(SELECT * FROM B) AS parent",
                                   "WHERE child.colX=parent.colY");
        }

        [Test]
        public void ReturnSimpleEffectiveSqlWithMultipleJoinConditions()
        {
            // given
            _parentTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM A");
            _referencedTriplesMap.Setup(tm => tm.EffectiveSqlQuery).Returns("SELECT * FROM B");

            // when
            _refObjectMap.AddJoinCondition("colX", "colY");
            _refObjectMap.AddJoinCondition("foo", "bar");
            _refObjectMap.AddJoinCondition("dlihc", "tnerap");

            // then
            AssertContainsSequence(_refObjectMap.EffectiveSQLQuery,
                                   "SELECT * FROM (SELECT * FROM A) AS child,",
                                   "(SELECT * FROM B) AS parent");
            Assert.IsTrue(_refObjectMap.EffectiveSQLQuery.Contains("child.colX=parent.colY"));
            Assert.IsTrue(_refObjectMap.EffectiveSQLQuery.Contains("child.foo=parent.bar"));
            Assert.IsTrue(_refObjectMap.EffectiveSQLQuery.Contains("child.dlihc=parent.tnerap"));
        }

        void AssertContainsSequence(string actualString, params string[] expectedValues)
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
