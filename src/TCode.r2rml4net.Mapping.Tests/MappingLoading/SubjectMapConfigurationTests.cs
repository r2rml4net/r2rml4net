using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class SubjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;

        [SetUp]
        public void Setup()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
        }

        [Test]
        public void CanInitizalieFromGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"".");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/employee/{EMPNO}", subjectMap.Template);
            Assert.AreEqual("http://www.example.com/triplesMap", ((IUriNode)subjectMap.ParentMapNode).Uri.ToString());
            Assert.AreEqual(graph.GetUriNode("ex:subject"), subjectMap.Node);
        }

        [Test]
        public void CanInitializeWithGraphMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graphMap [ rr:template ""http://data.example.com/jobgraph/{JOB}"" ] ;
	                                   rr:graphMap [ rr:constant <http://data.example.com/agraph/> ] .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.GetUriNode("ex:subject"), subjectMap.Node);
            Assert.AreEqual(2, subjectMap.GraphMaps.Count());
            Assert.AreEqual("http://data.example.com/jobgraph/{JOB}", subjectMap.GraphMaps.ElementAt(0).Template);
            Assert.AreEqual(new Uri("http://data.example.com/agraph/"), subjectMap.GraphMaps.ElementAt(1).URI);
            Assert.AreEqual(graph.GetBlankNode("autos1"), subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.GetBlankNode("autos2"), subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanInitializeWithShortcutGraphMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graph <http://data.example.com/shortGraph/> ;
	                                   rr:graph <http://data.example.com/agraph/> .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.GetUriNode("ex:subject"), subjectMap.Node);
            Assert.AreEqual(2, subjectMap.GraphMaps.Count());
            Assert.AreEqual(new Uri("http://data.example.com/shortGraph/"), subjectMap.GraphMaps.ElementAt(0).URI);
            Assert.AreEqual(new Uri("http://data.example.com/agraph/"), subjectMap.GraphMaps.ElementAt(1).URI);
            Assert.AreEqual(graph.GetBlankNode("autos1"), subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.GetBlankNode("autos2"), subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:TriplesMap rr:subject ex:Value .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:Value").Uri, subjectMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), subjectMap.Node);
        }
    }
}
