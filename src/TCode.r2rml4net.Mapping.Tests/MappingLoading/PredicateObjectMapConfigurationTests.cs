using System;
using System.Linq;
using NUnit.Framework;
using VDS.RDF;
using Moq;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class PredicateObjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<ITriplesMapConfiguration> _otherTriplesMap;
        private Mock<IR2RMLConfiguration> _configuration;

        [SetUp]
        public void Setup()
        {
            _otherTriplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _configuration =new Mock<IR2RMLConfiguration>();

            _configuration.Setup(config => config.TriplesMaps).Returns(new[] {_triplesMap.Object, _otherTriplesMap.Object});
            _triplesMap.Setup(tm => tm.R2RMLConfiguration).Returns(_configuration.Object);
        }

        [Test]
        public void CanBeInitializedWithPredicateMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap 
    rr:predicateMap [ rr:template ""http://data.example.com/employee/{EMPNO}"" ] ;
    rr:predicateMap [ rr:template ""http://data.example.com/user/{EMPNO}"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual("http://data.example.com/employee/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(0).Template);
            Assert.AreEqual("http://data.example.com/user/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(1).Template);
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.GetBlankNode("autos2"), predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithPredicateMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:predicate ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual(new Uri("http://www.example.com/Employee"), predicateObjectMap.PredicateMaps.ElementAt(0).URI);
            Assert.AreEqual(new Uri("http://www.example.com/Worker"), predicateObjectMap.PredicateMaps.ElementAt(1).URI);
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.GetBlankNode("autos2"), predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithGraphMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:graph ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.GraphMaps.Count());
            Assert.AreEqual(new Uri("http://www.example.com/Employee"), predicateObjectMap.GraphMaps.ElementAt(0).URI);
            Assert.AreEqual(new Uri("http://www.example.com/Worker"), predicateObjectMap.GraphMaps.ElementAt(1).URI);
            Assert.AreEqual(graph.GetBlankNode("autos1"), predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.GetBlankNode("autos2"), predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap, ex:PredicateObjectMap1 .
  
ex:PredicateObjectMap rr:objectMap 
    [ rr:constant ex:Employee ], 
    [ rr:template ""http://data.example.com/user/{EMPNO}"" ] .
  
ex:PredicateObjectMap1 rr:objectMap 
    [ rr:constant ex:Xxx ], 
    [ rr:template ""http://data.example.com/user/{xxx}"" ] .

ex:PredicateObjectMap rr:objectMap [
    rr:parentTriplesMap ex:TriplesMap2
] .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => new Uri("http://www.example.com/Employee").Equals(map.URI)));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => "http://data.example.com/user/{EMPNO}".Equals(map.Template)));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos1"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos2"))));
        }

        [Test]
        public void CanBeInitializedWithObjectMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:object ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Employee"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Worker"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos1"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos2"))));
        }

        [Test]
        public void CanBeInitializedWithObjectMapsUsingShortcutWhenBlankNodeIsUsed()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap _:blank .
_:blank rr:object ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetBlankNode("blank"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Employee"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Worker"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos1"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(graph.GetBlankNode("autos2"))));
        }

        [Test]
        public void CanBeInitializedWithRefObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap 
    [ rr:constant ex:Employee ], 
    [ rr:template ""http://data.example.com/user/{EMPNO}"" ] .

ex:PredicateObjectMap rr:objectMap ex:refObjectMap .
ex:refObjectMap rr:parentTriplesMap ex:TriplesMap2 .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.AreEqual(1, predicateObjectMap.RefObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:refObjectMap"), predicateObjectMap.RefObjectMaps.Cast<RefObjectMapConfiguration>().ElementAt(0).Node);
        }

        private const string InitialGraph = 
@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap _:blank .
_:blank rr:object ex:Employee, ex:Worker .";

        private const string ReplaceConstantsSparql =
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>

DELETE { ?map rr:object ?value . }
INSERT { ?map rr:objectMap [ rr:constant ?value ] . }
WHERE { ?map rr:object ?value }";

        private const string QuerySparql =
@"prefix ex: <http://www.example.com/>
prefix rr: <http://www.w3.org/ns/r2rml#>

select *
where
{
ex:triplesMap rr:predicateObjectMap ?map .
?map rr:constant ?value
}";

        const string ExpectedGraph =
@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap _:blank.
_:blank rr:objectMap _:autos1.
_:autos1 rr:constant ex:Employee.
_:autos2 rr:constant ex:Worker.
_:blank rr:objectMap _:autos2.";

        [Test]
        public void Test()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(InitialGraph);
            IGraph expectedGraph = new Graph();
            expectedGraph.LoadFromString(ExpectedGraph);

            // when
            TripleStore store = new TripleStore();
            store.Add(graph);

            var dataset = new InMemoryDataset(store, graph.BaseUri);
            ISparqlUpdateProcessor processor = new LeviathanUpdateProcessor(dataset);
            var updateParser = new SparqlUpdateParser();

            processor.ProcessCommandSet(updateParser.ParseFromString(ReplaceConstantsSparql));

            // then
            Assert.IsTrue(((SparqlResultSet)graph.ExecuteQuery(QuerySparql)).Any()); // this fails
        }
    }
}
