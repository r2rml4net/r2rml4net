using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
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
    }
}
