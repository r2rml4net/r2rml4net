using System;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<IPredicateObjectMapConfiguration> _predictaObjectMap;

        [SetUp]
        public void Setup()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _predictaObjectMap = new Mock<IPredicateObjectMapConfiguration>();
        }

        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap [ rr:template ""http://data.example.com/{JOB}"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, graph.GetBlankNode("autos1"));
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/{JOB}", objectMap.Template);
            Assert.AreEqual("http://www.example.com/PredicateObjectMap", ((IUriNode)objectMap.ParentMapNode).Uri.AbsoluteUri);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantIRIValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap [ rr:constant ex:someObject ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, graph.GetBlankNode("autos1"));
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantLiteralValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap [ rr:constant ""someObject"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, graph.GetBlankNode("autos1"));
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual("someObject", objectMap.Literal);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.Node);
        }

        [Test, Ignore("consider a way to allow directly passing a graph with shortcut node")]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:object ex:someObject .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.Node);
        }
    }
}