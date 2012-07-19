using System.Linq;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;
        private Mock<ITriplesMapConfiguration> _referencedTriplesMap;
        private Mock<IPredicateObjectMap> _predicateObjectMap;

        [SetUp]
        public void Setup()
        {
            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap = new Mock<ITriplesMapConfiguration>();
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
        }

        [Test]
        public void CanInitializeJoinConditions()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:TriplesMap rr:predicateObjectMap [
                                       rr:predicate ex:department;
                                       rr:objectMap [
                                           rr:parentTriplesMap ex:TriplesMap2;
                                           rr:joinCondition [
                                               rr:child ""DEPTNO"";
                                               rr:parent ""ID"";
                                           ];
                                       ];
                                   ].");
            _predicateObjectMap.Setup(map => map.Node).Returns(graph.GetBlankNode("autos1"));
            _parentTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap"));
            _referencedTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            _refObjectMap = new RefObjectMapConfiguration(_predicateObjectMap.Object, _parentTriplesMap.Object, _referencedTriplesMap.Object, graph);
            _refObjectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos2"));

            // then
            Assert.AreEqual(1, _refObjectMap.JoinConditions.Count());
            Assert.AreEqual("DEPTNO", _refObjectMap.JoinConditions.ElementAt(0).ChildColumn);
            Assert.AreEqual("ID", _refObjectMap.JoinConditions.ElementAt(0).ParentColumn);
            Assert.AreEqual(graph.GetBlankNode("autos2"), _refObjectMap.Node);
        }
    }
}
