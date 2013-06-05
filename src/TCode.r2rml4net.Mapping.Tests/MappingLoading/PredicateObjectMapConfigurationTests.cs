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
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class PredicateObjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<ITriplesMapConfiguration> _otherTriplesMap;
        private Mock<IR2RMLConfiguration> _configuration;

        [SetUp]
        public void Setup()
        {
            _otherTriplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _configuration = new Mock<IR2RMLConfiguration>();

            _configuration.Setup(config => config.TriplesMaps).Returns(new[] { _triplesMap.Object, _otherTriplesMap.Object });
            _triplesMap.Setup(tm => tm.R2RMLConfiguration).Returns(_configuration.Object);
        }

        [Test]
        public void CanBeInitializedWithPredicateMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap 
    rr:predicateMap [ rr:template ""http://data.example.com/employee/{EMPNO}"" ] ;
    rr:predicateMap [ rr:template ""http://data.example.com/user/{EMPNO}"" ].");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual("http://data.example.com/employee/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(0).Template);
            Assert.AreEqual("http://data.example.com/user/{EMPNO}", predicateObjectMap.PredicateMaps.ElementAt(1).Template);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(0).Object;
            Assert.AreEqual(blankNode1, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(1).Object;
            Assert.AreEqual(blankNode2, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithPredicateMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:predicate ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual(new Uri("http://www.example.com/Employee"), predicateObjectMap.PredicateMaps.ElementAt(0).URI);
            Assert.AreEqual(new Uri("http://www.example.com/Worker"), predicateObjectMap.PredicateMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(0).Object;
            Assert.AreEqual(blankNode1, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:predicateMap")).ElementAt(1).Object;
            Assert.AreEqual(blankNode2, predicateObjectMap.PredicateMaps.Cast<PredicateMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithGraphMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:graph ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.GraphMaps.Count());
            Assert.AreEqual(new Uri("http://www.example.com/Employee"), predicateObjectMap.GraphMaps.ElementAt(0).URI);
            Assert.AreEqual(new Uri("http://www.example.com/Worker"), predicateObjectMap.GraphMaps.ElementAt(1).URI);
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:graphMap")).ElementAt(0).Object;
            Assert.AreEqual(blankNode1, predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(0).Node);
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:graphMap")).ElementAt(1).Object;
            Assert.AreEqual(blankNode2, predicateObjectMap.GraphMaps.Cast<GraphMapConfiguration>().ElementAt(1).Node);
        }

        [Test]
        public void CanBeInitializedWithObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap, ex:PredicateObjectMap1 .
  
ex:PredicateObjectMap rr:objectMap 
    [ rr:constant ex:Employee ], 
    [ rr:template ""http://data.example.com/user/{EMPNO}"" ] .
  
ex:PredicateObjectMap1 rr:objectMap 
    [ rr:constant ex:Xxx ], 
    [ rr:template ""http://data.example.com/user/{xxx}"" ] .

ex:PredicateObjectMap rr:objectMap [
    rr:parentTriplesMap ex:TriplesMap2
] .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => new Uri("http://www.example.com/Employee").Equals(map.URI)));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => "http://data.example.com/user/{EMPNO}".Equals(map.Template)));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode1)));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode2)));
        }

        [Test]
        public void CanBeInitializedWithObjectMapsUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:object ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Employee"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Worker"))));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode1)));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode2)));
        }

        [Test]
        public void CanBeInitializedWithObjectMapsUsingShortcutWhenBlankNodeIsUsed()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap _:blank .
_:blank rr:object ex:Employee, ex:Worker .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetBlankNode("blank"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Employee"))));
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.URI.Equals(new Uri("http://www.example.com/Worker"))));
            var blankNode1 = graph.GetTriplesWithSubjectPredicate(graph.GetBlankNode("blank"), graph.CreateUriNode("rr:objectMap")).ElementAt(0).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode1)));
            var blankNode2 = graph.GetTriplesWithSubjectPredicate(graph.GetBlankNode("blank"), graph.CreateUriNode("rr:objectMap")).ElementAt(1).Object;
            Assert.IsTrue(predicateObjectMap.ObjectMaps.Any(map => map.Node.Equals(blankNode2)));
        }

        [Test]
        public void CanBeInitializedWithRefObjectMaps()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap 
    [ rr:constant ex:Employee ], 
    [ rr:template ""http://data.example.com/user/{EMPNO}"" ] .

ex:PredicateObjectMap rr:objectMap ex:refObjectMap .
ex:refObjectMap rr:parentTriplesMap ex:TriplesMap2 .");
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _otherTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph, graph.GetUriNode("ex:PredicateObjectMap"));
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(2, predicateObjectMap.ObjectMaps.Count());
            Assert.AreEqual(1, predicateObjectMap.RefObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:refObjectMap"), predicateObjectMap.RefObjectMaps.Cast<RefObjectMapConfiguration>().ElementAt(0).Node);
        }
    }
}
