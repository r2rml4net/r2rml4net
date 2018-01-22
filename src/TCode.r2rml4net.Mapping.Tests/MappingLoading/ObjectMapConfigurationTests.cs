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
using NUnit.Framework;
using Resourcer;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private Mock<ITriplesMapConfiguration> _triplesMap;
        private Mock<IPredicateObjectMapConfiguration> _predictaObjectMap;

        [SetUp]
        public void Setup()
        {
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _predictaObjectMap = new Mock<IPredicateObjectMapConfiguration>();
        }

        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.Simple.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual("http://data.example.com/{JOB}", objectMap.Template);
            Assert.AreEqual("http://www.example.com/PredicateObjectMap", ((IUriNode)objectMap.ParentMapNode).Uri.AbsoluteUri);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantIRIValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantIri.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithConstantLiteralValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantLiteral.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual("someObject", objectMap.Literal);
            Assert.IsNull(objectMap.Language);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithTypedLiteralValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantLiteralWithDatatype.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual("someObject", objectMap.Literal);
            Assert.AreEqual(new Uri("http://example.org/some#datatype"), objectMap.DataTypeURI);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithImplictlyTypedLiteralValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantLiteralWithDatatypeImplicit.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual("2", objectMap.Literal);
            Assert.AreEqual(new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger), objectMap.DataTypeURI);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test]
        public void CanBeInitializedWithLiteralValueWithLanguageTag()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantLiteralWithLanguage.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph, blankNode);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.IsTrue(((ITermMap)objectMap).IsConstantValued);
            Assert.AreEqual("someObject", objectMap.Literal);
            Assert.AreEqual("pl", objectMap.Language);
            Assert.AreEqual(blankNode, objectMap.Node);
        }

        [Test, Ignore("consider a way to allow directly passing a graph with shortcut node")]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.ObjectMap.ConstantIriShortcut.ttl"));
            _triplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:triplesMap"));
            _predictaObjectMap.Setup(map => map.Node).Returns(graph.GetUriNode("ex:PredicateObjectMap"));

            // when
            var objectMap = new ObjectMapConfiguration(_triplesMap.Object, _predictaObjectMap.Object, graph);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:PredicateObjectMap"), graph.CreateUriNode("rr:objectMap")).Single().Object;
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(blankNode, objectMap.Node);
        }
    }
}