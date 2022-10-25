﻿#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
//
// ------------------------------------------------------------------------
//
// This file is part of r2rml4net.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.
//
// ------------------------------------------------------------------------
//
// r2rml4net may alternatively be used under the LGPL licence
//
// http://www.gnu.org/licenses/lgpl.html
//
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using System.Data;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class RDFTermGeneratorTests
    {
        private RDFTermGenerator _termGenerator;
        private readonly Mock<ISubjectMap> _subjectMap;
        private Mock<IPredicateMap> _predicateMap;
        private readonly Mock<ISQLValuesMappingStrategy> _lexicalFormProvider;
        private readonly Mock<IDataRecord> _logicalRow;
        private Mock<ITermMap> _termMap;
        private readonly Mock<ITermType> _termType;
        private Mock<IGraphMap> _graphMap;
        private Mock<IObjectMap> _objectMap;

        public RDFTermGeneratorTests()
        {
            _objectMap = new Mock<IObjectMap>();
            _termMap = new Mock<ITermMap>();
            _termType = new Mock<ITermType>();
            _subjectMap = new Mock<ISubjectMap>();
            _logicalRow = new Mock<IDataRecord>(MockBehavior.Strict);
            _lexicalFormProvider = new Mock<ISQLValuesMappingStrategy>();

            _termMap.Setup(map => map.TermType).Returns(_termType.Object);

            _termGenerator = new RDFTermGenerator(new MappingOptions().WithBaseUri("http://example.com/base/"))
                                 {
                                     SqlValuesMappingStrategy = _lexicalFormProvider.Object,
                                 };
        }

        #region Test constant valued term map

        [Fact]
        public void SubjectMapsConstantIsAnIri()
        {
            // given
            _subjectMap.Setup(sm => sm.IsConstantValued).Returns(true);
            var uri = new Uri("http://www.example.com/const");
            _subjectMap.Setup(sm => sm.URI).Returns(uri);

            // when
            var node = _termGenerator.GenerateTerm<IUriNode>(_subjectMap.Object, _logicalRow.Object);

            // then
            Assert.Equal(uri, node.Uri);
        }

        [Fact]
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
            Assert.Equal(uri, node.Uri);
        }

        [Fact]
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
            Assert.Equal(uri, node.Uri);
        }

        [Fact]
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
            Assert.Equal(uri, node.Uri);
        }

        [Fact]
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

        [Fact]
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
            Assert.Equal(literal, node.Value);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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
            Assert.Null(node);
            _logicalRow.VerifyAll();
        }

        [Fact]
        public void ColumnNotFoundThrowsAndLogsError()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName))
                       .Throws<IndexOutOfRangeException>()
                       .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true);
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName);

            // when
            Assert.Throws<InvalidMapException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
        }

        #region IRI term type

        [Fact]
        public void RelativeUriValueCreatesUriNode()
        {
            const string expected = "http://example.com/base/value";

            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("value")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is IUriNode);
            Assert.Equal(expected, (node as IUriNode).Uri.AbsoluteUri);
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void InvalidUriValueThrowsException()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("../../value")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));

            // then
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Theory]
        [InlineData("http://example.com/company/Alice", "http://example.com/company/Alice")]
        [InlineData("Bob/Charles", "http://example.com/base/Bob/Charles")]
        [InlineData("Bob", "http://example.com/base/Bob")]
        public void EscapesColumnValuesBeforeConstructingAbsoluteUri(string value, string expected)
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                .Returns(value)
                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is IUriNode);
            Assert.Equal(expected, (node as IUriNode).Uri.AbsoluteUri);
        }

        [Fact]
        public void KeepsIriIntactWhenItIsAlreadyFullEncoded()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                .Returns("http://example.com/foo%20bar%20baz")
                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is IUriNode);
            Assert.Equal("http://example.com/foo%20bar%20baz", (node as IUriNode).Uri.AbsoluteUri);
        }

        [Theory]
        [InlineData("path/../Danny")]
        [InlineData("Bob Charles")]
        public void ThrowsWhenGeneratingTermFromInvalidValues(string value)
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                .Returns(value)
                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            Assert.Throws<InvalidTermException>(() =>
                _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object)
            );
        }

        [Fact]
        public void DoesntPercentEncodeValueAndThrowsExceptionForInvalidURI()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("Juan Daniel")
                                .Verifiable();
            _termMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));
        }

        #endregion

        #region Literal term type

        [Fact]
        public void GeneratesLiteralWithoutDatatypeOrLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is ILiteralNode);
            Assert.Equal("value", (node as ILiteralNode).Value);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void GeneratesLiteralWithDatatype()
        {
            // given
            const string expectedDatatype = "http://www.example.com/types/inch";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
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
            Assert.NotNull(node);
            Assert.True(node is ILiteralNode);
            Assert.Equal("value", (node as ILiteralNode).Value);
            Assert.Equal(expectedDatatype, (node as ILiteralNode).DataType.AbsoluteUri);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void GeneratesLiteralWithLanguageTag()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.Language).Returns("pl").Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is ILiteralNode);
            Assert.Equal("value", (node as ILiteralNode).Value);
            Assert.Equal("pl", (node as ILiteralNode).Language);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void CannotGenerateLiteralWithBothLanguageTagAndDatatype()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("value")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.Language).Returns("pl").Verifiable();
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

        [Fact]
        public void ExplicitDatatypeOverridesImplicit()
        {
            // given
            const string expectedDatatype = "http://www.example.com/types/inch";
            const string implicitDatatype = "http://www.example.com/types/cm";
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("value")
                                .Callback(() => datatype = new Uri(implicitDatatype))
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object).Verifiable();
            _objectMap.Setup(map => map.DataTypeURI).Returns(new Uri(expectedDatatype)).Verifiable();
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is ILiteralNode);
            Assert.Equal("value", (node as ILiteralNode).Value);
            Assert.Equal(expectedDatatype, (node as ILiteralNode).DataType.AbsoluteUri);
            _logicalRow.VerifyAll();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void DoesntPercentEncodeValue()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("Juan Daniel")
                                .Verifiable();
            _objectMap.Setup(map => map.IsColumnValued).Returns(true).Verifiable();
            _objectMap.Setup(map => map.ColumnName).Returns(ColumnName).Verifiable();
            _objectMap.Setup(map => map.TermType).Returns(_termType.Object);
            _termType.Setup(type => type.IsLiteral).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<ILiteralNode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.Equal("Juan Daniel", node.Value);
        }

        #endregion

        #endregion

        #region Test template-valued term map

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
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

        [Fact]
        public void NullValueReturnsNullNodeForTemplateTermMap()
        {
            // given
            _termMap = new Mock<ITermMap>();
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}");
            _termType.Setup(tt => tt.IsURI).Returns(false);
            _termMap.Setup(map => map.TermType).Returns(_termType.Object);

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.Null(node);
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
        }

        [Fact]
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
            _termType.Setup(tt => tt.IsURI).Returns(true);
            _termMap.Setup(map => map.TermType).Returns(_termType.Object);
            Uri typeUri;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, It.IsAny<IDataRecord>(), out typeUri)).Returns("value");

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.Null(node);
            _lexicalFormProvider.VerifyAll();
            _logicalRow.VerifyAll();
            _termMap.VerifyAll();
        }

        [Fact]
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
            _termType.Setup(tt => tt.IsURI).Returns(false).Verifiable();
            Uri type;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, It.IsAny<IDataRecord>(), out type)).Returns("5").Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(2, It.IsAny<IDataRecord>(), out type)).Returns("Tomasz Pluskiewicz").Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_objectMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is ILiteralNode);
            Assert.Equal("http://www.example.com/person/5/Tomasz Pluskiewicz", (node as ILiteralNode).Value);
            _logicalRow.VerifyAll();
            _logicalRow.Verify();
            _objectMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Fact]
        public void ReplacesAndEscapesColumnNameWithValuesForIRITemplatedTermMap()
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal("id")).Returns(1).Verifiable();
            _logicalRow.Setup(rec => rec.GetOrdinal("name")).Returns(2).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(It.IsAny<int>())).Returns(false).Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true);
            _termMap.Setup(map => map.Template).Returns("http://www.example.com/person/{id}/{name}");
            _termType.Setup(tt => tt.IsURI).Returns(true).Verifiable();
            Uri type;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(1, It.IsAny<IDataRecord>(), out type)).Returns("5").Verifiable();
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(2, It.IsAny<IDataRecord>(), out type)).Returns("Tomasz Pluskiewicz").Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is IUriNode);
            Assert.Equal(new Uri("http://www.example.com/person/5/Tomasz%20Pluskiewicz"), (node as IUriNode).Uri);
            _logicalRow.VerifyAll();
            _logicalRow.Verify();
            _termMap.VerifyAll();
            _termType.VerifyAll();
        }

        [Theory]
        [InlineData("http://example.com/company/Alice", "http://example.com/base/http%3A%2F%2Fexample.com%2Fcompany%2FAlice")]
        [InlineData("path/../Danny", "http://example.com/base/path%2F..%2FDanny")]
        [InlineData("Bob/Charles", "http://example.com/base/Bob%2FCharles")]
        [InlineData("Bob Charles", "http://example.com/base/Bob%20Charles")]
        public void EscapesColumnValuesBeforeComputingTemplateValue(string value, string expected)
        {
            // given
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex).Verifiable();
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns(value)
                                .Verifiable();
            _termMap.Setup(map => map.IsTemplateValued).Returns(true).Verifiable();
            _termMap.Setup(map => map.Template).Returns(string.Format("{{{0}}}", ColumnName)).Verifiable();
            _termType.Setup(type => type.IsURI).Returns(true).Verifiable();

            // when
            var node = _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
            Assert.True(node is IUriNode);
            Assert.Equal(expected, (node as IUriNode).Uri.AbsoluteUri);
        }

        [Fact]
        public void GeneratesValueForTemplateWithManyBraces()
        {
            // given
            var termMap = new Mock<ILiteralTermMap>();
            const string template = "\\{\\{\\{ {\"ISO 3166\"} \\}\\}\\}";
            _logicalRow.Setup(rec => rec.GetOrdinal("ISO 3166")).Returns(ColumnIndex);
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false);
            Uri datatype;
            _lexicalFormProvider.Setup(lex => lex.GetLexicalForm(ColumnIndex, It.IsAny<IDataRecord>(), out datatype))
                                .Returns("some value");
            termMap.Setup(map => map.IsTemplateValued).Returns(true);
            termMap.Setup(map => map.Template).Returns(template);
            termMap.Setup(map => map.TermType).Returns(_termType.Object);
            _termType.Setup(type => type.IsLiteral).Returns(true);

            // when
            var node = _termGenerator.GenerateTerm<ILiteralNode>(termMap.Object, _logicalRow.Object);

            // then
            Assert.Equal("{{{ some value }}}", node.Value);
        }

        #endregion

        [Fact]
        public void ThrowsWhenSubjectMapIsLiteral()
        {
            // given
            _subjectMap.Setup(map => map.IsColumnValued).Returns(true);
            _subjectMap.Setup(map => map.ColumnName).Returns(ColumnName);
            _subjectMap.Setup(map => map.TermType.IsLiteral).Returns(true);
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex);
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_subjectMap.Object, _logicalRow.Object));
        }

        [Fact]
        public void WhenOverridenInOptionsShouldAllowBlankSubjectNodesWithoutTemplateOrConstantOrColumn()
        {
            // given
            _subjectMap.Setup(map => map.TermType.IsBlankNode).Returns(true);
            var termGenerator = new RDFTermGenerator(new MappingOptions().WithBaseUri("http://example.com/base/").WithAutomaticBlankNodeSubjects())
            {
                SqlValuesMappingStrategy = _lexicalFormProvider.Object,
            };

            // when
            var node = termGenerator.GenerateTerm<IBlankNode>(_subjectMap.Object, _logicalRow.Object);

            // then
            Assert.NotNull(node);
        }

        [Fact]
        public void ThrowsWhenGraphMapIsNonIri()
        {
            // given
            _graphMap = new Mock<IGraphMap>();
            _graphMap.Setup(map => map.IsColumnValued).Returns(true);
            _graphMap.Setup(map => map.ColumnName).Returns(ColumnName);
            _graphMap.Setup(map => map.TermType.IsURI).Returns(false);
            _graphMap.Setup(map => map.TermType.IsLiteral).Returns(true);
            _logicalRow.Setup(rec => rec.GetOrdinal(ColumnName)).Returns(ColumnIndex);
            _logicalRow.Setup(rec => rec.IsDBNull(ColumnIndex)).Returns(false);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_graphMap.Object, _logicalRow.Object));
        }

        [Fact]
        public void ThrowsWhenTermIsNeitherColumnNorTemplateNorConstantValue()
        {
            // given
            _termMap.Setup(tm => tm.IsConstantValued).Returns(false);
            _termMap.Setup(tm => tm.IsTemplateValued).Returns(false);
            _termMap.Setup(tm => tm.IsColumnValued).Returns(false);

            // then
            Assert.Throws<InvalidTermException>(() => _termGenerator.GenerateTerm<INode>(_termMap.Object, _logicalRow.Object));
        }

        [Fact]
        public void RetrunsDifferentSubjectBlankNodesForSameValuesWhenPreservingDuplicateRows()
        {
            // given
            var termGenerator = new RDFTermGenerator(new MappingOptions().WithBaseUri("http://example.com/").WithDuplicateRowsPreserved());
            const string nodeId = "node id";
            _subjectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);

            // when
            var node = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
            var node2 = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);

            // then
            Assert.NotSame(node, node2);
        }

        [Fact]
        public void RetrunsSameSubjectBlankNodesForSameValuesWhenNotPreservingDuplicateRows()
        {
            // given
            const string nodeId = "node id";
            _subjectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);
            var termGenerator = new RDFTermGenerator(new MappingOptions().WithBaseUri("http://example.com/"));

            // when
            var node = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
            var node2 = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);

            // then
            Assert.Same(node, node2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReusesNodesForObjectsAndSubjectsWhenNotPreservingDulplicateRows(bool subjectFirst)
        {
            // given
            const string nodeId = "node id";
            _subjectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);
            _objectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);
            var termGenerator = new RDFTermGenerator(new MappingOptions().WithBaseUri("http://example.com/"));

            // when
            INode node2;
            INode node;
            if (subjectFirst)
            {
                node = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
                node2 = termGenerator.GenerateTermForValue(_objectMap.Object, nodeId);
            }
            else
            {
                node2 = termGenerator.GenerateTermForValue(_objectMap.Object, nodeId);
                node = termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
            }

            // then
            Assert.Same(node, node2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReusesNodesForObjectsAndSubjectsWhenPreservingDulplicateRows(bool subjectFirst)
        {
            // given
            const string nodeId = "node id";
            _subjectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);
            _objectMap.Setup(sm => sm.TermType.IsBlankNode).Returns(true);

            // when
            INode node2;
            INode node;
            if (subjectFirst)
            {
                node = _termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
                node2 = _termGenerator.GenerateTermForValue(_objectMap.Object, nodeId);
            }
            else
            {
                node2 = _termGenerator.GenerateTermForValue(_objectMap.Object, nodeId);
                node = _termGenerator.GenerateTermForValue(_subjectMap.Object, nodeId);
            }

            // then
            Assert.Same(node, node2);
        }
    }
}