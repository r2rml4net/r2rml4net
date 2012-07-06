using System;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private GraphMapConfiguration _graphMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _graphMap = new GraphMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void GraphMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _graphMap.IsConstantValued(uri);

            // then
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.ParentMapNode,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrGraphMapProperty)),
                    _graphMap.TermMapNode)));
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.TermMapNode,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _graphMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _graphMap.Graph);
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeLiteral()
        {
            _graphMap.TermType.IsLiteral();
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeBlankNode()
        {
            _graphMap.TermType.IsBlankNode();
        }

        [Test]
        public void GraphUriIsNullByDefault()
        {
            Assert.IsNull(_graphMap.Graph);
        }
        
        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graphMap [ rr:template ""http://data.example.com/jobgraph/{JOB}"" ].");

            // when
            _graphMap = new GraphMapConfiguration(graph.GetUriNode("ex:subject"), graph);
            _graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/jobgraph/{JOB}", _graphMap.Template);
            Assert.AreEqual("http://www.example.com/subject", ((IUriNode)_graphMap.ParentMapNode).Uri.ToString());
        }

        [Test]
        public void CanBeInitializedWithConstantValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graphMap [ rr:constant ex:graph ].");

            // when
            _graphMap = new GraphMapConfiguration(graph.GetUriNode("ex:subject"), graph);
            _graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, _graphMap.ConstantValue);
        }    

        [Test]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"";
	                                   rr:graph ex:graph .");

            // when
            _graphMap = new GraphMapConfiguration(graph.GetUriNode("ex:subject"), graph);
            _graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, _graphMap.ConstantValue);
        }

        [Test]
        public void InitializationWithoutGraphMapThrowsException()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:triplesMap rr:subjectMap ex:subject .
  
                                   ex:subject 
	                                   rr:template ""http://data.example.com/employee/{EMPNO}"" .");

            // when
            _graphMap = new GraphMapConfiguration(graph.GetUriNode("ex:subject"), graph);

            // then
            Assert.Throws<InvalidOperationException>(() => _graphMap.RecursiveInitializeSubMapsFromCurrentGraph());
        }
    }
}