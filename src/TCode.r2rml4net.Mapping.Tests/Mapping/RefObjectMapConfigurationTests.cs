using System;
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
            IUriNode referencedTriplesMap = graph.CreateUriNode(new Uri("http://test.example.com/TriplesMap"));
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            predicateObjectMap.Setup(map => map.Node).Returns(
                graph.CreateUriNode(new Uri("http://test.example.com/PredicateObjectMap")));

            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap.Setup(tm => tm.Node).Returns(referencedTriplesMap);

            _refObjectMap = new RefObjectMapConfiguration(_parentTriplesMap.Object, predicateObjectMap.Object, _referencedTriplesMap.Object, graph);
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
    }
}
