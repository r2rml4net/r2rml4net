using System;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    public class SubjectMapConfigurationTests
    {
        private SubjectMapConfiguration _subjectMapConfiguration;
        private IUriNode _triplesMapNode;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            _triplesMapNode = graph.CreateUriNode(new Uri("http://unittest.mappings.com/TriplesMap"));
            _subjectMapConfiguration = new SubjectMapConfiguration(_triplesMapNode, graph);
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

            // when
            _subjectMapConfiguration = new SubjectMapConfiguration(graph.GetUriNode("ex:triplesMap"), graph);
            _subjectMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetUriNode("ex:subject"));

            // then
            Assert.AreEqual("http://data.example.com/employee/{EMPNO}", _subjectMapConfiguration.Template);
            Assert.AreEqual("http://www.example.com/triplesMap", ((IUriNode)_subjectMapConfiguration.ParentMapNode).Uri.ToString());
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

            // when
            _subjectMapConfiguration = new SubjectMapConfiguration(graph.GetUriNode("ex:triplesMap"), graph);
            _subjectMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetUriNode("ex:subject"));

            // then
            Assert.AreEqual(2, _subjectMapConfiguration.Graphs.Count());
            Assert.AreEqual(1, _subjectMapConfiguration.Graphs.Count(g => g.Template == "http://data.example.com/jobgraph/{JOB}"));
            Assert.AreEqual(1, _subjectMapConfiguration.Graphs.Count(g => new Uri("http://data.example.com/agraph/").Equals(g.GraphUri)));
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

            // when
            _subjectMapConfiguration = new SubjectMapConfiguration(graph.GetUriNode("ex:triplesMap"), graph);
            _subjectMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetUriNode("ex:subject"));

            // then
            Assert.AreEqual(2, _subjectMapConfiguration.Graphs.Count());
            Assert.AreEqual(1, _subjectMapConfiguration.Graphs.Count(g => new Uri("http://data.example.com/shortGraph/").Equals(g.GraphUri)));
            Assert.AreEqual(1, _subjectMapConfiguration.Graphs.Count(g => new Uri("http://data.example.com/agraph/").Equals(g.GraphUri)));
        }

        [Test]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:TriplesMap rr:subject ex:Value .");

            // when
            var subjectMap = new SubjectMapConfiguration(graph.GetUriNode("ex:TriplesMap"), graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:Value").Uri, subjectMap.ConstantValue);
        }
    }
}
