using System;
using System.Linq;
using NUnit.Framework;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class PredicateObjectMapConfigurationTests
    {
        private PredicateObjectMapConfiguration _predicateObjectMap;
        private Uri _triplesMapURI;
        private Mock<ITriplesMapConfiguration> _triplesMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            _triplesMapURI = new Uri("http://tests.example.com/TriplesMap");
            var triplesMapNode = graph.CreateUriNode(_triplesMapURI);
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);
            _predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, new MappingOptions());
        }

        [Test]
        public void CreatingPredicateObjectMapCreatesTriple()
        {
            _predicateObjectMap.R2RMLMappings.VerifyHasTripleWithBlankObject(_triplesMapURI, UriConstants.RrPredicateObjectMapProperty);
        }

        [Test]
        public void CanCreateObjectMaps()
        {
            // when 
            var objectMap1 = _predicateObjectMap.CreateObjectMap();
            var objectMap2 = _predicateObjectMap.CreateObjectMap();

            // then
            Assert.AreNotSame(objectMap1, objectMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(objectMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(objectMap2);
            Assert.AreEqual(2, _predicateObjectMap.ObjectMaps.Count());
        }

        [Test]
        public void CanCreatePredicateMaps()
        {
            // when 
            var propertyMap1 = _predicateObjectMap.CreatePredicateMap();
            var propertyMap2 = _predicateObjectMap.CreatePredicateMap();

            // then
            Assert.AreNotSame(propertyMap1, propertyMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(propertyMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(propertyMap2);
        }

        [Test]
        public void CanCreateMultipleGraphMap()
        {
            // when
            IGraphMap graphMap1 = _predicateObjectMap.CreateGraphMap();
            IGraphMap graphMap2 = _predicateObjectMap.CreateGraphMap();

            // then
            Assert.AreNotSame(graphMap1, graphMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(graphMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(graphMap2);
        }

        [Test]
        public void CanCreateRefObjectMaps()
        {
            // given
            Mock<ITriplesMapConfiguration> parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            parentTriplesMap.Setup(tMap => tMap.Node).Returns(_predicateObjectMap.R2RMLMappings.CreateUriNode(new Uri("http://tests.example.com/OtherTriplesMap")));

            // when 
            var objectMap1 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
            var objectMap2 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);

            // then
            Assert.AreNotSame(objectMap1, objectMap2);
            Assert.IsInstanceOf<RefObjectMapConfiguration>(objectMap1);
            Assert.IsInstanceOf<RefObjectMapConfiguration>(objectMap2);
            Assert.AreEqual(2, _predicateObjectMap.RefObjectMaps.Count());
        }

        [Test]
        public void CanCreateObjectMapAndRefObjectMap()
        {
            // given
            Mock<ITriplesMapConfiguration> parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            parentTriplesMap.Setup(tMap => tMap.Node).Returns(_predicateObjectMap.R2RMLMappings.CreateUriNode(new Uri("http://tests.example.com/OtherTriplesMap")));

            // when
            _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
            _predicateObjectMap.CreateObjectMap();

            // then
            Assert.AreEqual(1, _predicateObjectMap.ObjectMaps.Count());
            Assert.AreEqual(1, _predicateObjectMap.RefObjectMaps.Count());
        }
    }
}
