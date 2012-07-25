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
        private Mock<ITermMap> _termMap;
        private Mock<ITermType> _termType;
        private Mock<IGraphMap> _graphMap;
        private Mock<IObjectMap> _objectMap;

        [SetUp]
        public void Setup()
        {
            _objectMap = new Mock<IObjectMap>();
            _termMap = new Mock<ITermMap>();
            _termType = new Mock<ITermType>();
            _logicalRow = new Mock<IDataRecord>(MockBehavior.Strict);
            _lexicalFormProvider = new Mock<INaturalLexicalFormProvider>();

            _termMap.Setup(map => map.TermType).Returns(_termType.Object);

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

        #region Test column-valued term maps

        private const string ColumnName = "Column";
        private const int ColumnIndex = 3;

        [Test]
        public void NullValueReturnsNullNode()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true);
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName);

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNull(node);
            _logicalRow.VerifyAll();
        }

        #region IRI term type

        [Test]
        public void AbsoluteUriValueCreatesUriNode()
        {
            // given
            const string expected = "http://www.example.com/value";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns(expected)
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is IUriNode);
            Assert.AreEqual(expected, (node as IUriNode).Uri.ToString());
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void RelativeUriValueCreatesUriNode()
        {
            const string expected = "http://www.example.com/value";

            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termMap.Setup(map => map.BaseURI).Returns(new Uri("http://www.example.com/")).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is IUriNode);
            Assert.AreEqual(expected, (node as IUriNode).Uri.ToString());
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void InvalidUriValueThrowsException()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("\\value")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termMap.Setup(map => map.BaseURI).Returns(new Uri("http://www.example.com/")).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        #endregion

        #region BlankNode term type

        [Test]
        public void ReturnsBlankNodeWithCustomIdentifier()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsBlankNode).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is IBlankNode);
            Assert.AreEqual("value", (node as IBlankNode).InternalID);
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        #endregion

        #region Literal term type

        [Test]
        public void GeneratesLiteralWithoutDatatypeOrLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is ILiteralNode);
            Assert.AreEqual("value", (node as ILiteralNode).Value);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void GeneratesLiteralWithDatatype()
        {
            // given
            const string expectedDatatype = "http://www.example.com/types/inch";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.DataTypeURI).Returns(new Uri(expectedDatatype)).Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is ILiteralNode);
            Assert.AreEqual("value", (node as ILiteralNode).Value);
            Assert.AreEqual(expectedDatatype, (node as ILiteralNode).DataType.ToString());
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void GeneratesLiteralWithLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.LanguageTag).Returns("pl").Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is ILiteralNode);
            Assert.AreEqual("value", (node as ILiteralNode).Value);
            Assert.AreEqual("pl", (node as ILiteralNode).Language);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void CannotGenerateLiteralWithBothLanguageTagAndDatatype()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetNaturalLexicalForm(ColumnIndex, _logicalRow.Object))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.LanguageTag).Returns("pl").Verifiable();
            _objectMap.Setup(map => map.DataTypeURI)
                      .Returns(new Uri("http://www.example.com/types/inch"))
                      .Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        #endregion

        #endregion
    }
}