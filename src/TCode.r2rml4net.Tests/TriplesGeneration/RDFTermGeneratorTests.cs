using System;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class RDFTermGeneratorTests
    {
        private RDFTermGenerator _termGenerator;
        private Mock<ISubjectMap> _subjectMap;
        private Mock<IPredicateMap> _predicateMap;
        private Mock<INaturalLexicalFormProvider> _lexicalFormProvider;
        private Mock<IDataRecord> _logicalRow;
        private Mock<IGraphMap> _graphMap;
        private Mock<IObjectMap> _objectMap;

        [SetUp]
        public void Setup()
        {
            _logicalRow = new Mock<IDataRecord>(MockBehavior.Strict);
            _lexicalFormProvider = new Mock<INaturalLexicalFormProvider>();
            _termGenerator = new RDFTermGenerator
                                 {
                                     LexicalFormProvider = _lexicalFormProvider.Object
                                 };
        }

        #region Test constant valued term map

        [Test]
        public void SubjectMapsConstantIsAnIri()
        {
            // given
            _subjectMap = new Mock<ISubjectMap>();
            _subjectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _subjectMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_subjectMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(uri, node.Uri);
        }

        [Test]
        public void PredicateMapsConstantIsAnIri()
        {
            // given
            _predicateMap = new Mock<IPredicateMap>();
            _predicateMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _predicateMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_predicateMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(uri, node.Uri);
        }

        [Test]
        public void GraphMapsConstantIsAnIri()
        {
            // given
            _graphMap = new Mock<IGraphMap>();
            _graphMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _graphMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_graphMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(uri, node.Uri);
        }

        [Test]
        public void ObjectMapsConstantCanBeIRI()
        {
            // given
            _objectMap = new Mock<IObjectMap>();
            _objectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _objectMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(uri, node.Uri);
        }

        [Test]
        public void UriValuedConstantMustHaveConstantSet()
        {
            // given
            var termMap = new Mock<IUriValuedTermMap>();
            termMap.Setup(sm => sm.IsConstantValued).Returns(true);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<IUriNode>(termMap.Object, _logicalRow.Object));
        }

        [Test]
        public void ObjectMapsConstantCanBeLiteral()
        {
            // given
            _objectMap = new Mock<IObjectMap>();
            _objectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            const string literal = "some value";
            _objectMap.Setup(sm => sm.Literal).Returns(literal);

            // when
            var node = _termGenerator.GenerateTerm<ILiteralNode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(literal, node.Value);
        }

        [Test]
        public void ObjectMapsConstantCannotBeBothLiteralAndIri()
        {
            // given
            _objectMap = new Mock<IObjectMap>();
            _objectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            const string literal = "some value";
            var uri = new Uri("http://www.example.com/const");
            _objectMap.Setup(sm => sm.Literal).Returns(literal);
            _objectMap.Setup(sm => sm.URI).Returns(uri);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object));
        }

        [Test]
        public void ObjectMapsConstantMusBeEitherLiteralOrIri()
        {
            // given
            _objectMap = new Mock<IObjectMap>();
            _objectMap.Setup(sm => sm.IsConstantValued).Returns(true);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object));
        }

        #endregion
    }
}