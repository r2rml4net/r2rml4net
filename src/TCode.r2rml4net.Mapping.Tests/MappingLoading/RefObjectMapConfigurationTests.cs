using System.Linq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;

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

            // when
            _refObjectMap = new RefObjectMapConfiguration(graph.GetBlankNode("autos1"), graph.GetUriNode("ex:TriplesMap2"), graph);
            _refObjectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos2"));

            // then
            Assert.AreEqual(1, _refObjectMap.JoinConditions.Count());
            Assert.AreEqual("DEPTNO", _refObjectMap.JoinConditions.ElementAt(0).ChildColumn);
            Assert.AreEqual("ID", _refObjectMap.JoinConditions.ElementAt(0).ParentColumn);
        }
    }
}
