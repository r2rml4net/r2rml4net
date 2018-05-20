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
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class PredicateMapConfigurationTests
    {
        private readonly IGraph _graph;
        private readonly PredicateMapConfiguration _predicateMap;
        private readonly Mock<ITriplesMapConfiguration> _triplesMapNode;
        private readonly Mock<IPredicateObjectMap> _predicateObjectMap;

        public PredicateMapConfigurationTests()
        {
            _graph = new FluentR2RML().R2RMLMappings;
            IUriNode triplesMapNode = _graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
            _predicateObjectMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode("predicateObjectMap"));

            _triplesMapNode = new Mock<ITriplesMapConfiguration>();
            _triplesMapNode.Setup(tm => tm.Node).Returns(triplesMapNode);

            _predicateMap = new PredicateMapConfiguration(_triplesMapNode.Object, _predicateObjectMap.Object, _graph);
        }

        [Fact]
        public void NodeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PredicateMapConfiguration(_triplesMapNode.Object, _predicateObjectMap.Object, _graph, null)
            );
        }

        [Fact]
        public void PredicateMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _predicateMap.IsConstantValued(uri);

            // then
            Assert.True(_predicateMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _predicateMap.ParentMapNode,
                    _predicateMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrPredicateMapProperty)),
                    _predicateMap.Node)));
            Assert.True(_predicateMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _predicateMap.Node,
                    _predicateMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _predicateMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.Equal(uri, _predicateMap.ConstantValue);
        }

        [Fact]
        public void PredicateMapCannotBeOfTypeLiteral()
        {
            Assert.Throws<InvalidMapException>(() => 
                _predicateMap.TermType.IsLiteral()
            );
        }

        [Fact]
        public void PredicateMapCannotBeOfTypeBlankNode()
        {
            Assert.Throws<InvalidMapException>(() =>
                _predicateMap.TermType.IsBlankNode()
            );
        }

        [Fact]
        public void PredicateIsNullByDefault()
        {
            Assert.Null(_predicateMap.URI);
        }

        [Fact]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.Equal(new Uri("http://www.w3.org/ns/r2rml#predicate"), _predicateMap.CreateShortcutPropertyNode().Uri);
        }
    }
}