using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class PredicateMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<IPredicateObjectMap> _predicateObjectMap;

        [SetUp]
        public void Setup()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
        }
            
        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:predicateMap [ rr:template ""http://data.example.com/employee/{EMPNO}"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));
            _predicateObjectMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var predicateMap = new PredicateMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, graph, graph.GetBlankNode("autos1"));
            predicateMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/employee/{EMPNO}", predicateMap.Template);
            Assert.AreEqual("http://www.example.com/PredicateObjectMap", ((IUriNode)predicateMap.ParentMapNode).Uri.ToString());
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:predicateMap [ rr:constant ex:Value ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));
            _predicateObjectMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var predicateMap = new PredicateMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, graph, graph.GetBlankNode("autos1"));
            predicateMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:Value").Uri, predicateMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateMap.Node);
        }

        [Test, Ignore("consider a way to allow directly passing a graph with shortcut node")]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:predicate ex:Value .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));
            _predicateObjectMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var predicateMap = new PredicateMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, graph);
            predicateMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:Value").Uri, predicateMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateMap.Node);
        }
    }
}