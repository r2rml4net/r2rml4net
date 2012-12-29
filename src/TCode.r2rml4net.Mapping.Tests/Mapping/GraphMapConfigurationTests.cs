#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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
using NUnit.Framework;
using TCode.r2rml4net.Exceptions;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private GraphMapConfiguration _graphMap;
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<IGraphMapParent> _predicateObjectMap;
        private IGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = _graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _predicateObjectMap = new Mock<IGraphMapParent>();
            _predicateObjectMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode("predicateObjectMap"));

            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);

            _graphMap = new GraphMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph, new MappingOptions());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NodeCannotBeNull()
        {
            _graphMap = new GraphMapConfiguration(_triplesMap.Object, _predicateObjectMap.Object, _graph, null);
        }

        [Test]
        public void GraphMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _graphMap.IsConstantValued(uri);

            // then
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.ParentMapNode,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrGraphMapProperty)),
                    _graphMap.Node)));
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.Node,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _graphMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _graphMap.URI);
        }

        [Test, ExpectedException(typeof (InvalidMapException))]
        public void GraphMapCannotBeOfTypeLiteral()
        {
            _graphMap.TermType.IsLiteral();
        }

        [Test, ExpectedException(typeof (InvalidMapException))]
        public void GraphMapCannotBeOfTypeBlankNode()
        {
            _graphMap.TermType.IsBlankNode();
        }

        [Test]
        public void GraphUriIsNullByDefault()
        {
            Assert.IsNull(_graphMap.URI);
        }

        [Test]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.AreEqual(new Uri("http://www.w3.org/ns/r2rml#graph"), _graphMap.CreateShortcutPropertyNode().Uri);
        }
    }
}