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
using System.Linq;
using Moq;
using NUnit.Framework;
using Resourcer;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<IGraphMapParent> _graphMapParent;

        [SetUp]
        public void Setup()
        {
            _graphMapParent = new Mock<IGraphMapParent>();
            _triplesMap = new Mock<ITriplesMapConfiguration>();
        }

        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.GraphMap.Simple.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _graphMapParent.Setup(map => map.Node).Returns(graph.GetUriNode("ex:subject"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).Single().Object;
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, _graphMapParent.Object, graph, blankNode);
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/jobgraph/{JOB}", graphMap.Template);
            Assert.AreEqual("http://www.example.com/subject", ((IUriNode) graphMap.ParentMapNode).Uri.AbsoluteUri);
            Assert.AreEqual(blankNode, graphMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.GraphMap.Constant.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _graphMapParent.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:subject"));
            
            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).Single().Object;
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, _graphMapParent.Object, graph, blankNode);
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, graphMap.ConstantValue);
            Assert.AreEqual(blankNode, graphMap.Node);
        }

        [Test, Ignore("consider a way to allow directly passing a graph with shortcut node")]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.GraphMap.ConstantShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _graphMapParent.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:subject"));

            // when
            var graphMap = new GraphMapConfiguration(_triplesMap.Object, _graphMapParent.Object, graph, graph.GetBlankNode("autos1"));
            graphMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:graph").Uri, graphMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), graphMap.Node);
        }
    }
}