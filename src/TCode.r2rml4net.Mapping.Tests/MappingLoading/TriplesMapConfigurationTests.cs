using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        private Mock<IR2RMLConfiguration> _configuration;
        private Mock<ISqlVersionValidator> _sqlVersionValidator;

        [SetUp]
        public void Setup()
        {
            _sqlVersionValidator = new Mock<ISqlVersionValidator>();
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
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));
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
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));
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
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));

            // then
            Assert.Throws<InvalidTriplesMapException>(triplesMap.RecursiveInitializeSubMapsFromCurrentGraph);
        }

        [Test]
        public void CanLoadMappingsWithManyPredicateObjectMaps()
        {
            // given
            IGraph mappings = new Graph();
            mappings.LoadFromEmbeddedResource("TCode.r2rml4net.Mapping.Tests.MappingLoading.RefObjectMap.ttl, TCode.r2rml4net.Mapping.Tests");
            var referencedMap = new TriplesMapConfiguration(CreateStub(mappings), mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap2")));
            var triplesMapConfiguration = new TriplesMapConfiguration(CreateStub(mappings), mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap1")));
            referencedMap.RecursiveInitializeSubMapsFromCurrentGraph();
            _configuration.Setup(c => c.TriplesMaps).Returns(new[] {referencedMap, triplesMapConfiguration});

            // when
            triplesMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(4, triplesMapConfiguration.PredicateObjectMaps.Count());
            Assert.AreEqual(3, triplesMapConfiguration.PredicateObjectMaps.Count(pm => !pm.RefObjectMaps.Any()));
            Assert.AreEqual(1, triplesMapConfiguration.PredicateObjectMaps.Count(pm => pm.RefObjectMaps.Any()));
        }

        [Test]
        public void CanBeCreatedFromTableName()
        {
            // given
            const string tableName = "SomeTable";
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;

            // when
            var triplesMap = TriplesMapConfiguration.FromTable(CreateStub(graph), tableName);

            // then
            Assert.AreEqual(tableName, triplesMap.TableName);
        }

        private TriplesMapConfigurationStub CreateStub(IGraph graph)
        {
            return new TriplesMapConfigurationStub(_configuration.Object,
                graph, new MappingOptions(), _sqlVersionValidator.Object);
        }
    }
}