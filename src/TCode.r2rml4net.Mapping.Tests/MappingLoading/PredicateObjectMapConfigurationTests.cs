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
using Xunit;
using Resourcer;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    public class PredicateObjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<ITriplesMapConfiguration> _otherTriplesMap;
        private Mock<IR2RMLConfiguration> _configuration;

        public PredicateObjectMapConfigurationTests()
        {
            _otherTriplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _configuration = new Mock<IR2RMLConfiguration>();

            _configuration.Setup(config => config.TriplesMaps).Returns(new[] { _triplesMap.Object, _otherTriplesMap.Object });
            _triplesMap.Setup(tm => tm.R2RMLConfiguration).Returns(_configuration.Object);
        }

        [Fact]
        public void CanBeInitializedWithPredicateMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.Simple.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.PredicateMaps.Count());
            Assert.Equal("http://data.example.com/employee/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(0).Template);
            Assert.Equal("http://data.example.com/user/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(1).Template);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(0).Object;
            Assert.Equal(blankNode1, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(1).Object;
            Assert.Equal(blankNode2, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Fact]
        public void CanBeInitializedWithPredicateMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.PredicateShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.PredicateMaps.Count());
            Assert.Equal(new Uri("http://www.example.com/Employee"), predicateObjectMap.PredicateMaps.ElementAt(0).URI);
            Assert.Equal(new Uri("http://www.example.com/Worker"), predicateObjectMap.PredicateMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(0).Object;
            Assert.Equal(blankNode1, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(1).Object;
            Assert.Equal(blankNode2, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Fact]
        public void CanBeInitializedWithGraphMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.GraphShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.GraphMaps.Count());
            Assert.Equal(new Uri("http://www.example.com/Employee"), predicateObjectMap.GraphMaps.ElementAt(0).URI);
            Assert.Equal(new Uri("http://www.example.com/Worker"), predicateObjectMap.GraphMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:graphMap")).ElementAt(0).Object;
            Assert.Equal(blankNode1, predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:graphMap")).ElementAt(1).Object;
            Assert.Equal(blankNode2, predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Fact]
        public void CanBeInitializedWithObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.ObjectMaps.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.ObjectMaps.Count());
            Assert.Contains(predicateObjectMap.ObjectMaps, map => new Uri("http://www.example.com/Employee").Equals(map.URI));
            Assert.Contains(predicateObjectMap.ObjectMaps, map => "http://data.example.com/user/{EMPNO}".Equals(map.Template));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode1));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode2));
        }

        [Fact]
        public void CanBeInitializedWithObjectMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.ObjectShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.ObjectMaps.Count());
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.URI.Equals(new Uri("http://www.example.com/Employee")));
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.URI.Equals(new Uri("http://www.example.com/Worker")));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode1));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode2));
        }

        [Fact]
        public void CanBeInitializedWithObjectMapsUsingShortcutWhenBlankNodeIsUsed()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.Blank.ObjectShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetBlankNode("blank"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.ObjectMaps.Count());
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.URI.Equals(new Uri("http://www.example.com/Employee")));
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.URI.Equals(new Uri("http://www.example.com/Worker")));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetBlankNode("blank"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode1));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetBlankNode("blank"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.Contains(predicateObjectMap.ObjectMaps, map => map.Node.Equals(blankNode2));
        }

        [Fact]
        public void CanBeInitializedWithRefObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.PredicateObjectMap.RefObjectMap.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.Equal(2, predicateObjectMap.ObjectMaps.Count());
            Assert.Single(predicateObjectMap.RefObjectMaps);
            Assert.Equal(graph.CreateUriNode("ex:refObjectMap"), predicateObjectMap.RefObjectMaps.Cast<RefObjectMapConfiguration>().ElementAt(0).Node);
        }
    }
}
