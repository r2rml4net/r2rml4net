using System;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class PredicateObjectMapConfigurationTests
    {
        private PredicateObjectMapConfiguration _predicateObjectMap;
        private Uri _triplesMapURI;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            _triplesMapURI = new Uri("http://tests.example.com/TriplesMap");
            var triplesMapNode = graph.CreateUriNode(_triplesMapURI);
            _predicateObjectMap = new PredicateObjectMapConfiguration(triplesMapNode, graph);
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
            parentTriplesMap.Setup(tMap => tMap.Uri).Returns(new Uri("http://tests.example.com/OtherTriplesMap"));

            // when 
            var objectMap1 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
            var objectMap2 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);

            // then
            Assert.AreNotSame(objectMap1, objectMap2);
            Assert.IsInstanceOf<RefObjectMapConfiguration>(objectMap1);
            Assert.IsInstanceOf<RefObjectMapConfiguration>(objectMap2);
            Assert.AreEqual(2, _predicateObjectMap.RefObjectMaps.Count());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CannotCreateObjectMapAndRefObjectMap(bool refMapFirst)
        {
            // given
            Mock<ITriplesMapConfiguration> parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            parentTriplesMap.Setup(tMap => tMap.Uri).Returns(new Uri("http://tests.example.com/OtherTriplesMap"));

            if (refMapFirst)
            {
                _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
                Assert.Throws<InvalidTriplesMapException>(() => _predicateObjectMap.CreateObjectMap());
            }
            else
            {
                _predicateObjectMap.CreateObjectMap();
                Assert.Throws<InvalidTriplesMapException>(() => _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object));
            }
        }
    }
}
