#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        private Mock<IR2RMLConfiguration> _configuration;
        private Mock<ISqlVersionValidator> _sqlVersionValidator;

        [SetUp]
        public void Setup()
        {
            _sqlVersionValidator = new Mock<ISqlVersionValidator>();
            _configuration = new Mock<IR2RMLConfiguration>();
        }

        [Test]
        public void CanBeInitizalizedFromGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.TriplesMap.Simple.ttl"));

            // when
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));
            triplesMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(graph.GetUriNode("ex:subject"), ((SubjectMapConfiguration)triplesMap.SubjectMap).Node);
            Assert.AreEqual(graph.GetUriNode("ex:triplesMap"), triplesMap.Node);
            Assert.AreEqual(3, triplesMap.PredicateObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:predObj1"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj2"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(1).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj3"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(2).Node);
        }

        [Test]
        public void CanBeInitizalizedFromGraphWithShortcutSubject()
        {
            // given
            IGraph graph = new Graph(); 
            graph.LoadFromString(Resource.AsString("Graphs.TriplesMap.SubjectShortcut.ttl"));

            // when
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));
            triplesMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            var blankNode = graph.GetTriplesWithSubjectPredicate(graph.GetUriNode("ex:triplesMap"), graph.CreateUriNode("rr:subjectMap")).ElementAt(0).Object;
            Assert.AreEqual(blankNode, ((SubjectMapConfiguration)triplesMap.SubjectMap).Node);
            Assert.AreEqual(new Uri("http://www.example.com/subject"), triplesMap.SubjectMap.URI);
            Assert.AreEqual(graph.GetUriNode("ex:triplesMap"), triplesMap.Node);
            Assert.AreEqual(3, triplesMap.PredicateObjectMaps.Count());
            Assert.AreEqual(graph.CreateUriNode("ex:predObj1"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(0).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj2"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(1).Node);
            Assert.AreEqual(graph.CreateUriNode("ex:predObj3"), triplesMap.PredicateObjectMaps.Cast<PredicateObjectMapConfiguration>().ElementAt(2).Node);
        }

        [Test]
        public void CanBeInitizalizedFromGraphWithMultipleSubjects()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(Resource.AsString("Graphs.TriplesMap.MultipleSubjects.ttl"));

            // when
            var triplesMap = new TriplesMapConfiguration(CreateStub(graph), graph.CreateUriNode("ex:triplesMap"));

            // then
            Assert.Throws<InvalidMapException>(triplesMap.RecursiveInitializeSubMapsFromCurrentGraph);
        }

        [Test]
        public void CanLoadMappingsWithManyPredicateObjectMaps()
        {
            // given
            IGraph mappings = new Graph();
            mappings.LoadFromString(Resource.AsString("Graphs.RefObjectMap.Complex.ttl"));
            var referencedMap = new TriplesMapConfiguration(CreateStub(mappings), mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap2")));
            var triplesMapConfiguration = new TriplesMapConfiguration(CreateStub(mappings), mappings.GetUriNode(new Uri("http://example.com/base/TriplesMap1")));
            referencedMap.RecursiveInitializeSubMapsFromCurrentGraph();
            _configuration.Setup(c => c.TriplesMaps).Returns(new[] {referencedMap, triplesMapConfiguration});

            // when
            triplesMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(4, triplesMapConfiguration.PredicateObjectMaps.Count());
            Assert.AreEqual(3, triplesMapConfiguration.PredicateObjectMaps.Count(pm => !pm.RefObjectMaps.Any()));
            Assert.AreEqual(1, triplesMapConfiguration.PredicateObjectMaps.Count(pm => pm.RefObjectMaps.Any()));
        }

        [Test]
        public void CanBeCreatedFromTableName()
        {
            // given
            const string tableName = "SomeTable";
            IGraph graph = new FluentR2RML().R2RMLMappings;

            // when
            var triplesMap = TriplesMapConfiguration.FromTable(CreateStub(graph), tableName);

            // then
            Assert.AreEqual(tableName, triplesMap.TableName);
        }

        private TriplesMapConfigurationStub CreateStub(IGraph graph)
        {
            return new TriplesMapConfigurationStub(_configuration.Object, graph, _sqlVersionValidator.Object);
        }
    }
}