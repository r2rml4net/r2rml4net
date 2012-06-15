using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private ObjectMapConfiguration _objectMap;
        private Uri _tripesMapURI;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            _tripesMapURI = new Uri("http://test.example.com/TestMapping");
            IUriNode triplesMapNode = graph.CreateUriNode(_tripesMapURI);
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
                    _objectMap.ParentMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty)),
                    _objectMap.TermMapNode)));
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(literal))));
            Assert.AreEqual(literal, _objectMap.Literal);
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
                    _objectMap.ParentMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty)),
                    _objectMap.TermMapNode)));
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _objectMap.Object);
        }

        [Test]
        public void ObjectMapLiteralConstantCanBeTyped()
        {
            // given
            const string literal = "5";

            // when
            _objectMap.IsConstantValued(literal).HasDataType(new Uri(UriConstants.RdfInteger));

            // then

            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrDataTypeProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RdfInteger)))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermType.GetURI().ToString());
            Assert.IsEmpty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))).Count());
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.TermMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))).Count());
        }

        [Test]
        public void ObjectMapLiteralConstantCanHaveLanguagTag()
        {
            // given
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguageTag("pl");

            // then
            AssertLanguageTag("pl");
        }

        private void AssertLanguageTag(string languagTagValue)
        {
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.TermMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrLanguageTagProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(languagTagValue))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermType.GetURI().ToString());
            Assert.IsEmpty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))).Count());
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.TermMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))).Count());
        }

        [Test]
        public void ObjectMapLiteralConstantLanguagTagCanBeSetUsingCultureInfo()
        {
            // given
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguageTag(new CultureInfo("pl-PL"));

            // then
            AssertLanguageTag("pl-pl");
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
            Assert.Throws<InvalidTriplesMapException>(() => literalConfiguration.HasLanguageTag("pl-PL"));
        }

        [Test]
        public void DefaultValuesAreNull()
        {
            IObjectMap map = _objectMap;
            Assert.IsNull(map.ColumnName);
            Assert.IsNull(map.Literal);
            Assert.IsNull(map.Object);
            Assert.IsNull(map.Template);
        }
    }
}