#region Licence
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
using System.Globalization;
using System.Linq;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class ObjectMapConfigurationTests
    {
        private ObjectMapConfiguration _objectMap;
        private Uri _tripesMapURI;
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private IGraph _graph;
        private Mock<IPredicateObjectMapConfiguration> _predicateObjectMap;
        private Mock<ILanguageTagValidator> _languageTagValidator;

        public ObjectMapConfigurationTests()
        {
            _graph = new FluentR2RML().R2RMLMappings;
            _tripesMapURI = new Uri("http://test.example.com/TestMapping");
            IUriNode triplesMapNode = _graph.CreateUriNode(_tripesMapURI);
            _predicateObjectMap = new Mock<IPredicateObjectMapConfiguration>();
            _predicateObjectMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode("predicateObjectMap"));

            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);

            _languageTagValidator = new Mock<ILanguageTagValidator>(MockBehavior.Strict);

            _objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph)
                {
                    LanguageTagValidator = _languageTagValidator.Object
                };
        }

        [Fact]
        public void NodeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph, null)
            );
        }

        [Fact]
        public void ObjectMapCanBeLiteralConstantValued()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.ParentMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty)),
                    _objectMap.Node)));
            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(literal))));
            Assert.Equal(literal, _objectMap.Literal);
        }

        [Fact]
        public void ConstantLiteralValueCanBeSetOnlyOnce()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            Assert.Throws<InvalidMapException>(() => _objectMap.IsConstantValued(literal));
        }

        [Fact]
        public void ObjectMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _objectMap.IsConstantValued(uri);

            // then
            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.ParentMapNode,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty)),
                    _objectMap.Node)));
            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.Equal(uri, _objectMap.URI);
        }

        [Fact]
        public void ObjectMapLiteralConstantCanBeTyped()
        {
            // given
            const string literal = "5";

            // when
            _objectMap.IsConstantValued(literal).HasDataType(new Uri(UriConstants.RdfInteger));

            // then

            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrDataTypeProperty)),
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RdfInteger)))));
            Assert.Equal(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
            Assert.Empty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.Single(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))));
            Assert.Single(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.Node,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))));
        }

        [Fact]
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
            Assert.True(_objectMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _objectMap.Node,
                    _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrLanguageProperty)),
                    _objectMap.R2RMLMappings.CreateLiteralNode(languagTagValue))));
            Assert.Equal(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
            Assert.Empty(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectProperty))));
            Assert.Single(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.ParentMapNode,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrObjectMapProperty))));
            Assert.Single(_objectMap.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _objectMap.Node,
                _objectMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty))));
        }

        [Fact]
        public void CannotSetInvalidLanguageTag()
        {
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("english")).Returns(false);

            Assert.Throws<ArgumentException>(() =>
                _objectMap.HasLanguage("english")
            );
        }

        [Fact]
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

        [Fact]
        public void CannotSetBothLanguageAndDataType()
        {
            // given
            const string literal = "some text";
            _languageTagValidator.Setup(validator => validator.LanguageTagIsValid("pl-PL")).Returns(true);
            var literalConfiguration = _objectMap.IsConstantValued(literal);

            // when
            literalConfiguration.HasLanguage(new CultureInfo("pl-PL"));

            // then
            Assert.Throws<InvalidMapException>(() => literalConfiguration.HasDataType(UriConstants.RdfInteger));
            Assert.Throws<InvalidMapException>(() => literalConfiguration.HasLanguage("pl-PL"));
        }

        [Fact]
        public void DefaultValuesAreNull()
        {
            IObjectMap map = _objectMap;
            Assert.Null(map.ColumnName);
            Assert.Null(map.Literal);
            Assert.Null(map.URI);
            Assert.Null(map.Template);
        }

        [Fact]
        public void CanBeOfTermTypeLiteral()
        {
            // when
            _objectMap.TermType.IsLiteral();

            // then
            Assert.Equal(UriConstants.RrLiteral, _objectMap.TermTypeURI.AbsoluteUri);
        }

        [Fact]
        public void TermTypeCannotBeSetTwice()
        {
            // when
            _objectMap.TermType.IsLiteral();

            // then
            Assert.Throws<InvalidMapException>(() => _objectMap.TermType.IsBlankNode());
            Assert.Throws<InvalidMapException>(() => _objectMap.TermType.IsIRI());
            Assert.Throws<InvalidMapException>(() => _objectMap.TermType.IsLiteral());
        }

        [Fact]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.Equal(new Uri("http://www.w3.org/ns/r2rml#object"), _objectMap.CreateShortcutPropertyNode().Uri);
        }
    }
}