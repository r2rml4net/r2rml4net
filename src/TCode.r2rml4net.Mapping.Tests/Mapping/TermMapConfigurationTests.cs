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
using System.Linq;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class TermMapConfigurationTests
    {
        private INode _triplesMapNode;
        private Mock<TermMapConfiguration> _termMapConfigurationMock;
        private TermMapConfiguration _termMapConfiguration;
        private IGraph _graph;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;

        public TermMapConfigurationTests()
        {
            _graph = new FluentR2RML().R2RMLMappings;
            _triplesMapNode = _graph.CreateUriNode(new Uri("http://mapping.com/SomeMap"));
            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _parentTriplesMap.Setup(tm => tm.Node).Returns(_triplesMapNode);
            Mock<IMapBase> parentMap = new Mock<IMapBase>();
            parentMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode());

            _termMapConfigurationMock = new Mock<TermMapConfiguration>(_parentTriplesMap.Object, parentMap.Object, _graph, _graph.CreateBlankNode())
                                            {
                                                CallBase = true
                                            };
            _termMapConfigurationMock
                .Setup(config => config.CreateMapPropertyNode())
                .Returns(_graph.CreateUriNode(new Uri(UriConstants.RrSubjectMapProperty)));
            _termMapConfiguration = _termMapConfigurationMock.Object;
        }

        [Fact]
        public void ConstantIRIValueCanBeSetOnlyOnce()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _termMapConfiguration.IsConstantValued(uri);

            // then
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.IsConstantValued(uri));
        }

        [Fact]
        public void CanBeConstantIRIValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _termMapConfiguration.IsConstantValued(uri);

            // then
            Assert.Equal(uri, _termMapConfiguration.ConstantValue);
            Assert.True(((ITermMap)_termMapConfiguration).IsConstantValued);
        }

        [Fact]
        public void TermMapCanBeColumnValued()
        {
            // given
            const string columnName = "Name";

            // when
            _termMapConfiguration.IsColumnValued(columnName);

            // then
            Assert.True(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode,
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.True(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrColumnProperty)),
                _termMapConfiguration.R2RMLMappings.CreateLiteralNode(columnName))));
            Assert.Equal(UriConstants.RrIRI, _termMapConfiguration.TermTypeURI.AbsoluteUri);
            Assert.Equal(columnName, _termMapConfiguration.ColumnName);
            Assert.True(((ITermMap)_termMapConfiguration).IsColumnValued);
        }

        [Fact]
        public void ColumnValueCanOnlyBeSetOnce()
        {
            // given
            const string columnName = "Name";

            // when
            _termMapConfiguration.IsColumnValued(columnName);

            // then
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.IsColumnValued(columnName));
        }

        [Fact]
        public void TermMapCanBeTemplateValued()
        {
            // given
            const string template = @"\\{\\{\\{ \\\\o/ {TITLE} \\\\o/ \\}\\}\\}";

            // when
            _termMapConfiguration.IsTemplateValued(template);

            //then
            Assert.True(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode,
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.True(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrTemplateProperty)),
                _termMapConfiguration.R2RMLMappings.CreateLiteralNode(template))));
            Assert.Equal(UriConstants.RrIRI, _termMapConfiguration.TermTypeURI.AbsoluteUri);
            Assert.Equal(template, _termMapConfiguration.Template);
            Assert.True(((ITermMap)_termMapConfiguration).IsTemplateValued);
        }

        [Fact]
        public void TemplateCanOnlyBeSetOnce()
        {
            // given
            const string template = @"\\{\\{\\{ \\\\o/ {TITLE} \\\\o/ \\}\\}\\}";

            // when
            _termMapConfiguration.IsTemplateValued(template);

            // then
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.IsTemplateValued(template));
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.IsTemplateValued("something else"));
        }

        [Fact]
        public void CanBeOfTypeBlankNode()
        {
            // when
            _termMapConfiguration.TermType.IsBlankNode();

            // then
            Assert.True(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode, 
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.True(_termMapConfiguration.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrTermTypeProperty))).Any());
            Assert.Equal(UriConstants.RrBlankNode, _termMapConfiguration.TermTypeURI.AbsoluteUri);
            Assert.True((_termMapConfiguration as ITermType).IsBlankNode);
            Assert.False((_termMapConfiguration as ITermType).IsURI);
            Assert.False((_termMapConfiguration as ITermType).IsLiteral);
        }

        [Fact]
        public void ColumnNameIsNullByDefault()
        {
            Assert.Null(_termMapConfiguration.ColumnName);
        }

        [Fact]
        public void TemplateIsNullByDefault()
        {
            Assert.Null(_termMapConfiguration.Template);
        }

        [Fact]
        public void ConstantValueIsNullByDefault()
        {
            Assert.Null(_termMapConfiguration.ConstantValue);
        }

        [Fact]
        public void CanHaveInverseExpression()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.SetInverseExpression(expression);

            // then
            Assert.Equal(expression, _termMapConfiguration.InverseExpression);
        }

        [Fact]
        public void CannotHaveInverseExpressionWhenConstantValued()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.IsConstantValued(new Uri("http://www.example.com/TermUri"));

            // then
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.SetInverseExpression(expression));
        }

        [Fact]
        public void CannotConstantValueWhenInverseExpressionIsSet()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.SetInverseExpression(expression);

            // then
            Assert.Throws<InvalidMapException>(() => _termMapConfiguration.IsConstantValued(new Uri("http://www.example.com/TermUri")));
        }

        [Fact]
        public void TermTypeIsIriByDefault()
        {
            ITermType type = _termMapConfiguration;
            Assert.True(type.IsURI);
            Assert.False(type.IsLiteral);
            Assert.False(type.IsBlankNode);
        }
    }
}
