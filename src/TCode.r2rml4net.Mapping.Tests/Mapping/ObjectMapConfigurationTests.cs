using System;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private ObjectMapConfiguration _objectMap;
        private Uri _tripesMapURI;
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private IGraph _graph;
        private Mock<IPredicateObjectMapConfiguration> _predicateObjectMap;
        private Mock<ILanguageTagValidator> _languageTagValidator;

        [SetUp]
        public void Setup()
        {
            _graph = new R2RMLConfiguration().R2RMLMappings;
            _tripesMapURI = new Uri("http://test.example.com/TestMapping");
            IUriNode triplesMapNode = _graph.CreateUriNode(_tripesMapURI);
            _predicateObjectMap = new Mock<IPredicateObjectMapConfiguration>();
            _predicateObjectMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode("predicateObjectMap"));

            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);

            _languageTagValidator = new Mock<ILanguageTagValidator>(MockBehavior.Strict);

            _objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph, new MappingOptions())
                {
                    LanguageTagValidator = _languageTagValidator.Object
                };
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NodeCannotBeNull()
        {
            _objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph, null, new MappingOptions());
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
                    _objectMap.Node)));
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
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
                    _objectMap.Node)));
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _objectMap.URI);
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
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrDataTypeProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RdfInteger)))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
            Assert.IsEmpty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))).Count());
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.Node,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))).Count());
        }

        [Test]
        public void ObjectMapLiteralConstantCanHaveLanguagTag()
        {
            // given
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("pl")).Returns(true);
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguage("pl");

            // then
            AssertLanguage("pl");
        }

        private void AssertLanguage(string languagTagValue)
        {
            Assert.IsTrue(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrLanguageProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(languagTagValue))));
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
            Assert.IsEmpty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))).Count());
            Assert.AreEqual(1, _objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.Node,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))).Count());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void CannotSetInvalidLanguageTag()
        {
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("english")).Returns(false);
            _objectMap.HasLanguage("english");
        }

        [Test]
        public void ObjectMapLiteralConstantLanguagTagCanBeSetUsingCultureInfo()
        {
            // given
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("pl-PL")).Returns(true);
            const string literal = "some text";

            // when
            _objectMap.IsConstantValued(literal).HasLanguage(new CultureInfo("pl-PL"));

            // then
            AssertLanguage("pl-pl");
        }

        [Test]
        public void CannotSetBothLanguageAndDataType()
        {
            // given
            const string literal = "some text";
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("pl-PL")).Returns(true);
            var literalConfiguration = _objectMap.IsConstantValued(literal);

            // when
            literalConfiguration.HasLanguage(new CultureInfo("pl-PL"));

            // then
            Assert.Throws<InvalidTriplesMapException>(() => literalConfiguration.HasDataType(UriConstants.RdfInteger));
            Assert.Throws<InvalidTriplesMapException>(() => literalConfiguration.HasLanguage("pl-PL"));
        }

        [Test]
        public void DefaultValuesAreNull()
        {
            IObjectMap map = _objectMap;
            Assert.IsNull(map.ColumnName);
            Assert.IsNull(map.Literal);
            Assert.IsNull(map.URI);
            Assert.IsNull(map.Template);
        }

        [Test]
        public void CanBeOfTermTypeLiteral()
        {
            // when
            _objectMap.TermType.IsLiteral();

            // then
            Assert.AreEqual(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
        }

        [Test]
        public void TermTypeCannotBeSetTwice()
        {
            // when
            _objectMap.TermType.IsLiteral();

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _objectMap.TermType.IsBlankNode());
            Assert.Throws<InvalidTriplesMapException>(() => _objectMap.TermType.IsIRI());
            Assert.Throws<InvalidTriplesMapException>(() => _objectMap.TermType.IsLiteral());
        }

        [Test]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.AreEqual(new Uri("http://www.w3.org/ns/r2rml#object"), _objectMap.CreateShortcutPropertyNode().Uri);
        }
    }
}