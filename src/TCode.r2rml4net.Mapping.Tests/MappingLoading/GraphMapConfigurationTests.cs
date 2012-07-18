using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;

        [SetUp]
        public void Setup()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
        }

        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graphMap [ rr:template ""http://data.example.com/jobgraph/{JOB}"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:subject"));

            // when
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, graph.GetUriNode("ex:subject"), graph);
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual("http://data.example.com/jobgraph/{JOB}", graphMap.Template);
            Assert.AreEqual("http://www.example.com/subject", ((IUriNode) graphMap.ParentMapNode).Uri.ToString());
            Assert.AreEqual(graph.GetBlankNode("autos1"), graphMap.ConfigurationNode);
        }

        [Test]
        public void CanBeInitializedWithConstantValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graphMap [ rr:constant ex:graph ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:subject"));

            // when
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, graph.GetUriNode("ex:subject"), graph);
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, graphMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), graphMap.ConfigurationNode);
        }

        [Test]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graph ex:graph .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:subject"));

            // when
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, graph.GetUriNode("ex:subject"), graph);
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph(graphMap.R2RMLMappings.GetBlankNode("autos1"));

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, graphMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), graphMap.ConfigurationNode);
        }
    }
}