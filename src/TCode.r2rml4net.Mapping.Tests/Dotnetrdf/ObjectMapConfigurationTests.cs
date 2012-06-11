using System;
using System.Globalization;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private ObjectMapConfiguration _objectMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _objectMap = new ObjectMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void ObjectMapCanBeLiteralConstantValued()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TriplesMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(literal))));
        }

        [Test]
        public void ConstantLiteralValueCanBeSetOnlyOnce()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _objectMap.IsConstantValued(literal));
        }

        [Test]
        public void ObjectMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _objectMap.IsConstantValued(uri);

            // then
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TriplesMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(uri))));
        }

        [Test]
        public void ObjectMapLiteralConstantCanBeTyped()
        {
            // given
            const string literal = "5";

            // when
            _objectMap.IsConstantValued(literal).HasDataType(UriConstants.RdfInteger);

            // then
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrDataTypeProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RdfInteger)))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermType.URI.ToString());
        }

        [Test]
        public void ObjectMapLiteralConstantCanHaveLanguagTag()
        {
            // given
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguageTag("pl");

            // then
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrLanguageTagProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode("pl"))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermType.URI.ToString());
        }

        [Test]
        public void ObjectMapLiteralConstantLanguagTagCanBeSetUsingCultureInfo()
        {
            // given
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguageTag(new CultureInfo("pl-PL"));

            // then
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrLanguageTagProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode("pl-pl"))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermType.URI.ToString());
        }

        [Test]
        public void CannotSetBothLanguageTagAndDataType()
        {
            // given
            const string literal = "some text";
            var literalConfiguration = _objectMap.IsConstantValued(literal);

            // when
            literalConfiguration.HasLanguageTag(new CultureInfo("pl-PL"));

            // then
            Assert.Throws<InvalidTriplesMapException>(() => literalConfiguration.HasDataType(UriConstants.RdfInteger));
        }
    }
}