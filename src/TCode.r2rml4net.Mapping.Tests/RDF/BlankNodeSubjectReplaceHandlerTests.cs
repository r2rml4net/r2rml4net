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
using System.Collections.Generic;
using Moq;
using Xunit;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.RDF
{
    public class BlankNodeSubjectReplaceHandlerTests
    {
        private readonly BlankNodeSubjectReplaceHandler _handler;
        private readonly Mock<IRdfHandler> _decoratedHandler;

        public BlankNodeSubjectReplaceHandlerTests()
        {
            _decoratedHandler = new Mock<IRdfHandler>(MockBehavior.Strict);
            _handler = new BlankNodeSubjectReplaceHandler(_decoratedHandler.Object);
            _decoratedHandler.Setup(h => h.StartRdf());
            _handler.StartRdf();
        }

        [Fact]
        public void ReplacesBlankNodeSubjectOnce()
        {
            // given
            var newBlankNode = MockNode<IBlankNode>();
            var blankNode = new Mock<IBlankNode>();
            blankNode.Setup(b => b.InternalID).Returns("some invalid identifier");
            IList<INode> subjects = new List<INode>();
            Triple triple = new Triple(blankNode.Object, MockNode<IUriNode>(), MockNode<ILiteralNode>());
            Triple triple2 = new Triple(blankNode.Object, MockNode<IUriNode>(), MockNode<ILiteralNode>());
            _decoratedHandler.Setup(h => h.HandleTriple(It.IsAny<Triple>()))
                             .Callback((Triple t) => subjects.Add(t.Subject))
                             .Returns(true);
            _decoratedHandler.Setup(h => h.CreateBlankNode()).Returns(newBlankNode);

            // when
            _handler.HandleTriple(triple);
            _handler.HandleTriple(triple2);

            // then
            foreach (var subject in subjects)
            {
                Assert.NotNull(subject);
                Assert.Same(newBlankNode, subject);
            }
            _decoratedHandler.Verify(h=>h.CreateBlankNode(), Times.Once());
        }

        [Fact]
        public void TestDoesNotReplaceUriSubject()
        {
            // given
            var subj = MockNode<IUriNode>();
            var subj1 = MockNode<IUriNode>();
            Triple triple = new Triple(subj, MockNode<IUriNode>(), MockNode<ILiteralNode>());
            Triple triple1 = new Triple(subj1, MockNode<IUriNode>(), MockNode<ILiteralNode>());
            _decoratedHandler.Setup(h => h.HandleTriple(It.IsAny<Triple>())).Returns(true);

            // when
            _handler.HandleTriple(triple);

            // then
            Assert.Same(subj, triple.Subject);
            Assert.Same(subj1, triple1.Subject);
        }

        [Fact]
        public void AlsoReplacesBlankNodeObjects()
        {
            // given
            var newBlankNode = MockNode<IBlankNode>();
            var blankNode = new Mock<IBlankNode>();
            blankNode.Setup(b => b.InternalID).Returns("some invalid identifier");
            IList<Triple> triples = new List<Triple>();
            Triple triple = new Triple(blankNode.Object, MockNode<IUriNode>(), MockNode<ILiteralNode>());
            Triple triple2 = new Triple(MockNode<IUriNode>(), MockNode<IUriNode>(), blankNode.Object);
            _decoratedHandler.Setup(h => h.HandleTriple(It.IsAny<Triple>()))
                             .Callback((Triple t) => triples.Add(t))
                             .Returns(true);
            _decoratedHandler.Setup(h => h.CreateBlankNode()).Returns(newBlankNode);

            // when
            _handler.HandleTriple(triple);
            _handler.HandleTriple(triple2);

            // then
            _decoratedHandler.Verify(h => h.CreateBlankNode(), Times.Once()); 
            Assert.Equal(triples[0].Subject, triples[1].Object);
        }

        private TNode MockNode<TNode>() where TNode : class, INode
        {
            var mock = new Mock<TNode>();
            return mock.Object;
        }
    }
}