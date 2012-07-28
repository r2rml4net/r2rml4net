using System;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Log;
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
        private Mock<ILexicalFormProvider> _lexicalFormProvider;
        private Mock<IDataRecord> _logicalRow;
        private Mock<ITermMap> _termMap;
        private Mock<ITermType> _termType;
        private Mock<IGraphMap> _graphMap;
        private Mock<IObjectMap> _objectMap;
        private Mock<IRDFTermGenerationLog> _log;

        [SetUp]
        public void Setup()
        {
            _log = new Mock<IRDFTermGenerationLog>();
            _objectMap = new Mock<IObjectMap>();
            _termMap = new Mock<ITermMap>();
            _termType = new Mock<ITermType>();
            _subjectMap = new Mock<ISubjectMap>();
            _logicalRow = new Mock<IDataRecord>(MockBehavior.Strict);
            _lexicalFormProvider = new Mock<ILexicalFormProvider>();

            _termMap.Setup(map => map.TermType).Returns(_termType.Object);

            _termGenerator = new RDFTermGenerator
                                 {
                                     LexicalFormProvider = _lexicalFormProvider.Object,
                                     Log = _log.Object
                                 };
        }

        #region Test constant valued term map

        [Test]
        public void SubjectMapsConstantIsAnIri()
        {
            // given
            _subjectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _subjectMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_subjectMap.Object, _logicalRow.Object);

            // then
            Assert.AreEqual(uri, node.Uri);
            _log.Verify(log => log.LogTermGenerated(node));
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
            _log.Verify(log => log.LogTermGenerated(node));
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
            _log.Verify(log => log.LogTermGenerated(node));
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void UriValuedConstantMustHaveConstantSet()
        {
            // given
            var termMap = new Mock<IUriValuedTermMap>();
            termMap.Setup(sm => sm.IsConstantValued).Returns(true);

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<IUriNode>(termMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
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
            _log.Verify(log => log.LogTermGenerated(node));
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

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
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
            _log.Verify(log => log.LogNullTermGenerated(_termMap.Object));
        }

        [Test]
        public void ColumnNotFoundReturnsNullNodeAndLogsError()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName))
                       .Throws<IndexOutOfRangeException>()
                       .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true);
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName);

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNull(node);
            _logicalRow.VerifyAll();
            _log.Verify(log => log.LogColumnNotFound(_termMap.Object, ColumnName));
            _log.Verify(log => log.LogNullTermGenerated(_termMap.Object));
        }

        #region IRI term type

        [Test]
        public void AbsoluteUriValueCreatesUriNode()
        {
            // given
            const string expected = "http://www.example.com/value";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void RelativeUriValueCreatesUriNode()
        {
            const string expected = "http://www.example.com/value";

            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void InvalidUriValueThrowsException()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        #endregion

        #region Literal term type

        [Test]
        public void GeneratesLiteralWithoutDatatypeOrLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void GeneratesLiteralWithDatatype()
        {
            // given
            const string expectedDatatype = "http://www.example.com/types/inch";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void GeneratesLiteralWithLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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
            _log.Verify(log => log.LogTermGenerated(node));
        }

        [Test]
        public void CannotGenerateLiteralWithBothLanguageTagAndDatatype()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, _logicalRow.Object))
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

        #region Test template-valued term map

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ThrowsIsTemplateIsNotSet(string template)
        {
            // given
            _termMap = new Mock<ITermMap>();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns(template);

            // when
            Assert.Throws<InvalidTemplateException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));

            // then
            _termMap.VerifyAll();
        }

        [Test]
        public void NullValueReturnsNullNodeForTemplateTermMap()
        {
            // given
            _termMap = new Mock<ITermMap>();
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}");

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNull(node);
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _log.Verify(log => log.LogNullTermGenerated(_termMap.Object));
        }

        [Test]
        public void AnyNullValueReturnsNullNodeForTemplateTermMap()
        {
            // given
            _termMap = new Mock<ITermMap>();
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(1).Verifiable();
            _logicalRow.Setup(rec => rec.GetOrdinal("name")).Returns(2).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(1)).Returns(false).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(2)).Returns(true).Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}/{name}");

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNull(node);
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _log.Verify(log => log.LogNullTermGenerated(_termMap.Object));
        }

        [Test]
        public void ReplacesColumnNameWithValuesForLiteralTemplatedTermMap()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(1).Verifiable();
            _logicalRow.Setup(rec => rec.GetOrdinal("name")).Returns(2).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(It.IsAny<int>())).Returns(false).Verifiable();
            _objectMap.Setup(map => map.IsTemplateValued).Returns(true);
            _objectMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}/{name}");
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object);
            _termType.Setup(tt => tt.IsLiteral).Returns(true).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, _logicalRow.Object)).Returns("5").Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(2, _logicalRow.Object)).Returns("Tomasz Pluskiewicz").Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is ILiteralNode);
            Assert.AreEqual("http://www.example.com/person/5/Tomasz Pluskiewicz", (node as ILiteralNode).Value);
            _logicalRow.VerifyAll();
            _logicalRow.Verify();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void ReplacesColumnNamesWithValuesForBlankNodeTemplatedTermMap()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(1).Verifiable();
            _logicalRow.Setup(rec => rec.GetOrdinal("name")).Returns(2).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(It.IsAny<int>())).Returns(false).Verifiable();
            _objectMap.Setup(map => map.IsTemplateValued).Returns(true);
            _objectMap.Setup(map => map.Template).Returns("person_{id}_{name}");
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object);
            _termType.Setup(tt => tt.IsBlankNode).Returns(true).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, _logicalRow.Object)).Returns("5").Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(2, _logicalRow.Object)).Returns("Pluskiewicz").Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is IBlankNode);
            Assert.AreEqual("person_5_Pluskiewicz", (node as IBlankNode).InternalID);
            _logicalRow.VerifyAll();
            _logicalRow.Verify();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Test]
        public void ReplacesAndEscapesColumnNameWithValuesForIRITemplatedTermMap()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(1).Verifiable();
            _logicalRow.Setup(rec => rec.GetOrdinal("name")).Returns(2).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(It.IsAny<int>())).Returns(false).Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}/{name}");
            _termType.Setup(tt => tt.IsURI).Returns(true).Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, _logicalRow.Object)).Returns("5").Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(2, _logicalRow.Object)).Returns("Tomasz Pluskiewicz").Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.IsNotNull(node);
            Assert.IsTrue(node is IUriNode);
            Assert.AreEqual(new Uri("http://www.example.com/person/5/Tomasz%20Pluskiewicz"), (node as IUriNode).Uri);
            _logicalRow.VerifyAll();
            _logicalRow.Verify();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        #endregion

        [TestCase("0number_first")]
        [TestCase("has spaces")]
        [TestCase("-non_letter_first")]
        public void LogsPotentiallyInvalidBlankNodeIds(string identifier)
        {
            // given
            _termMap.Setup(map => map.TermType.IsBlankNode).Returns(true);

            // when
            _termGenerator.GenerateTermForValue(_termMap.Object, identifier);

            // then
            _log.Verify(log => log.LogInvalidBlankNode(_termMap.Object, identifier));
        }
    }
}