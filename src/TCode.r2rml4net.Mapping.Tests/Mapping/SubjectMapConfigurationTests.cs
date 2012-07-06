using System;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
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
        public void CanAddMultipleClassIrisToSubjectMap()
        {
            // given
            Uri class1 = new Uri("http://example.com/ontology#class");
            Uri class2 = new Uri("http://example.com/ontology#anotherClass");
            Uri class3 = new Uri("http://semantic.mobi/resource/yetAnother");

            // when
            _subjectMapConfiguration.AddClass(class1).AddClass(class2).AddClass(class3);

            // then
            Assert.AreEqual(3, _subjectMapConfiguration.ClassIris.Length);
            Assert.Contains(class1, _subjectMapConfiguration.ClassIris);
            Assert.Contains(class2, _subjectMapConfiguration.ClassIris);
            Assert.Contains(class3, _subjectMapConfiguration.ClassIris);
        }

        [Test]
        public void CanSetTermMapsTermTypeToIRI()
        {
            // when
            _subjectMapConfiguration.TermType.IsIRI();

            // then
            Assert.AreEqual(UriConstants.RrIRI, _subjectMapConfiguration.TermType.GetURI().ToString());
            _subjectMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrTermTypeProperty, UriConstants.RrIRI);
        }

        [Test]
        public void CanSetTermMapsTermTypeToBlankNode()
        {
            // when
            _subjectMapConfiguration.TermType.IsBlankNode();

            // then
            Assert.AreEqual(UriConstants.RrBlankNode, _subjectMapConfiguration.TermType.GetURI().ToString());
            _subjectMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrTermTypeProperty, UriConstants.RrBlankNode);
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void CannnotSetTermMapsTermTypeToLiteral()
        {
            // when
            _subjectMapConfiguration.TermType.IsLiteral();
        }

        [Test]
        public void DefaultTermTypeIsIRI()
        {
            Assert.AreEqual(UriConstants.RrIRI, _subjectMapConfiguration.URI.ToString());
        }

        [Test]
        public void CannoSetTermTypeTwice()
        {
            // when
            _subjectMapConfiguration.TermType.IsIRI();

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _subjectMapConfiguration.TermType.IsIRI());
            Assert.Throws<InvalidTriplesMapException>(() => _subjectMapConfiguration.TermType.IsBlankNode());
        }

        [Test]
        public void SubjectMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _subjectMapConfiguration.IsConstantValued(uri);

            // then
            Assert.IsTrue(_subjectMapConfiguration.R2RMLMappings.ContainsTriple(
                new Triple(
                    _subjectMapConfiguration.ParentMapNode,
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrSubjectMapProperty)),
                    _subjectMapConfiguration.TermMapNode)));
            Assert.IsTrue(_subjectMapConfiguration.R2RMLMappings.ContainsTriple(
                new Triple(
                    _subjectMapConfiguration.TermMapNode,
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _subjectMapConfiguration.Subject);
        }

        [Test]
        public void CanHaveClassesAndBeTemplateValued()
        {
            // given
            Uri class1 = new Uri("http://example.com/ontology#class");
            string template = "http://www.example.com/res/{column}";

            // when
            _subjectMapConfiguration.AddClass(class1).IsTemplateValued(template);

            // then
            Assert.Contains(class1, _subjectMapConfiguration.ClassIris);
            Assert.AreEqual(template, _subjectMapConfiguration.Template);
        }

        [Test]
        public void SubjectIsNullByDefault()
        {
            Assert.IsNull(_subjectMapConfiguration.Subject);
        }

        [Test]
        public void CanCreateMultipleGraphMap()
        {
            // when
            IGraphMap graphMap1 = _subjectMapConfiguration.CreateGraphMap();
            IGraphMap graphMap2 = _subjectMapConfiguration.CreateGraphMap();

            // then
            Assert.AreNotSame(graphMap1, graphMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(graphMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(graphMap2);
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
    }
}
