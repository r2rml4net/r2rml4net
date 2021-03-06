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
using System.Linq;
using Moq;
using Xunit;
using Resourcer;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    public class SubjectMapConfigurationTests
    {
        private readonly Mock<ITriplesMapConfiguration> _triplesMap;

        public SubjectMapConfigurationTests()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
        }

        [Fact]
        public void CanInitizalieFromGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.SubjectMap.Simple.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:subject"));
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal("http://data.example.com/employee/{EMPNO}", subjectMap.Template);
            Assert.Equal("http://www.example.com/triplesMap", ((IUriNode)subjectMap.ParentMapNode).Uri.AbsoluteUri);
            Assert.Equal(graph.GetUriNode("ex:subject"), subjectMap.Node);
        }

        [Fact]
        public void CanInitializeWithGraphMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.SubjectMap.GraphMaps.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:subject"));
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(graph.GetUriNode("ex:subject"), subjectMap.Node);
            Assert.Equal(2, subjectMap.GraphMaps.Count());
            Assert.Equal("http://data.example.com/jobgraph/{JOB}", subjectMap.GraphMaps.ElementAt(0).Template);
            Assert.Equal(new Uri("http://data.example.com/agraph/"), subjectMap.GraphMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).ElementAt(0).Object;
            Assert.Equal(blankNode1, subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).ElementAt(1).Object;
            Assert.Equal(blankNode2, subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Fact]
        public void CanInitializeWithShortcutGraphMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.SubjectMap.GraphMapsShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:subject"));
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(graph.GetUriNode("ex:subject"), subjectMap.Node);
            Assert.Equal(2, subjectMap.GraphMaps.Count());
            Assert.Equal(new Uri("http://data.example.com/shortGraph/"), subjectMap.GraphMaps.ElementAt(0).URI);
            Assert.Equal(new Uri("http://data.example.com/agraph/"), subjectMap.GraphMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).ElementAt(0).Object;
            Assert.Equal(blankNode1, subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:subject"), graph.CreateUriNode("rr:graphMap")).ElementAt(1).Object;
            Assert.Equal(blankNode2, subjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [SkippableFact(Skip = "consider a way to allow directly passing a graph with shortcut node")]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.SubjectMap.Shortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var subjectMap = new SubjectMapConfiguration(_triplesMap.Object, graph);
            subjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(graph.CreateUriNode("ex:Value").Uri, subjectMap.ConstantValue);
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:TriplesMap"), graph.CreateUriNode("rr:subjectMap")).ElementAt(1).Object;
            Assert.Equal(blankNode, subjectMap.Node);
        }
    }
}
