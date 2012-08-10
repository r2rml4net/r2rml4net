using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        private Mock<IR2RMLConfiguration> _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IR2RMLConfiguration>();
        }

        [Test]
        public void CanBeInitizalizedFromGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:subjectMap ex:subject .
ex:triplesMap rr:predicateObjectMap ex:predObj1, ex:predObj2, ex:predObj3 .");

            // when
            var triplesMap = new TriplesMapConfiguration(_configuration.Object, graph, graph.CreateUriNode("ex:triplesMap"));
            triplesMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.GetUriNode("ex:subject"), ((SubjectMapConfiguration)triplesMap.SubjectMap).Node);
            Assert.AreEqual(graph.GetUriNode("ex:triplesMap"), triplesMap.Node);
            Assert.AreEqual(3, triplesMap.PredicateObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:predObj1"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj2"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(1).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj3"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(2).Node);
        }

        [Test]
        public void CanBeInitizalizedFromGraphWithShortcutSubject()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:subject ex:subject .
ex:triplesMap rr:predicateObjectMap ex:predObj1, ex:predObj2, ex:predObj3 .");

            // when
            var triplesMap = new TriplesMapConfiguration(_configuration.Object, graph, graph.CreateUriNode("ex:triplesMap"));
            triplesMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.GetBlankNode("autos1"), ((SubjectMapConfiguration)triplesMap.SubjectMap).Node);
            Assert.AreEqual(new Uri("http://www.example.com/subject"), triplesMap.SubjectMap.URI);
            Assert.AreEqual(graph.GetUriNode("ex:triplesMap"), triplesMap.Node);
            Assert.AreEqual(3, triplesMap.PredicateObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:predObj1"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj2"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(1).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj3"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(2).Node);
        }

        [Test]
        public void CanBeInitizalizedFromGraphWithMultipleSubjects()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:subject ex:subject .
ex:triplesMap rr:subjectMap ex:subject1 .");

            // when
            var triplesMap = new TriplesMapConfiguration(_configuration.Object, graph, graph.CreateUriNode("ex:triplesMap"));

            // then
            Assert.Throws<InvalidTriplesMapException>(triplesMap.RecursiveInitializeSubMapsFromCurrentGraph);
        }

        [Test]
        public void CanLoadMappingsWithManyPredicateObjectMaps()
        {
            // given
            IGraph mappings = new Graph();
            mappings.LoadFromEmbeddedResource("TCode.r2rml4net.Mapping.Tests.MappingLoading.RefObjectMap.ttl, TCode.r2rml4net.Mapping.Tests");
            var referencedMap = new TriplesMapConfiguration(_configuration.Object, mappings, mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap2")));
            var triplesMapConfiguration = new TriplesMapConfiguration(_configuration.Object, mappings, mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap1")));
            referencedMap.RecursiveInitializeSubMapsFromCurrentGraph();
            _configuration.Setup(c => c.TriplesMaps).Returns(new[] {referencedMap, triplesMapConfiguration});

            // when
            triplesMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(4, triplesMapConfiguration.PredicateObjectMaps.Count());
            Assert.AreEqual(3, triplesMapConfiguration.PredicateObjectMaps.Count(pm => !pm.RefObjectMaps.Any()));
            Assert.AreEqual(1, triplesMapConfiguration.PredicateObjectMaps.Count(pm => pm.RefObjectMaps.Any()));
        }
    }
}