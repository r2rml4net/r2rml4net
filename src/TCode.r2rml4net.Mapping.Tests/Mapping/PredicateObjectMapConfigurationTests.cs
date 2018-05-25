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
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class PredicateObjectMapConfigurationTests
    {
        private readonly PredicateObjectMapConfiguration _predicateObjectMap;
        private readonly Uri _triplesMapURI;
        private readonly Mock<ITriplesMapConfiguration> _triplesMap;

        public PredicateObjectMapConfigurationTests()
        {
            IGraph graph = new FluentR2RML().R2RMLMappings;
            _triplesMapURI = new Uri("http://tests.example.com/TriplesMap");
            var triplesMapNode = graph.CreateUriNode(_triplesMapURI);
            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);
            _predicateObjectMap = new PredicateObjectMapConfiguration(_triplesMap.Object, graph);
        }

        [Fact]
        public void CreatingPredicateObjectMapCreatesTriple()
        {
            _predicateObjectMap.R2RMLMappings.VerifyHasTripleWithBlankObject(_triplesMapURI, UriConstants.RrPredicateObjectMapProperty);
        }

        [Fact]
        public void CanCreateObjectMaps()
        {
            // when 
            var objectMap1 = _predicateObjectMap.CreateObjectMap();
            var objectMap2 = _predicateObjectMap.CreateObjectMap();

            // then
            Assert.NotSame(objectMap1, objectMap2);
            Assert.True(objectMap1 is TermMapConfiguration);
            Assert.True(objectMap2 is TermMapConfiguration);
            Assert.Equal(2, _predicateObjectMap.ObjectMaps.Count());
        }

        [Fact]
        public void CanCreatePredicateMaps()
        {
            // when 
            var propertyMap1 = _predicateObjectMap.CreatePredicateMap();
            var propertyMap2 = _predicateObjectMap.CreatePredicateMap();

            // then
            Assert.NotSame(propertyMap1, propertyMap2);
            Assert.True(propertyMap1 is TermMapConfiguration);
            Assert.True(propertyMap2 is TermMapConfiguration);
        }

        [Fact]
        public void CanCreateMultipleGraphMap()
        {
            // when
            IGraphMap graphMap1 = _predicateObjectMap.CreateGraphMap();
            IGraphMap graphMap2 = _predicateObjectMap.CreateGraphMap();

            // then
            Assert.NotSame(graphMap1, graphMap2);
            Assert.True(graphMap1 is TermMapConfiguration);
            Assert.True(graphMap2 is TermMapConfiguration);
        }

        [Fact]
        public void CanCreateRefObjectMaps()
        {
            // given
            Mock<ITriplesMapConfiguration> parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            parentTriplesMap.Setup(tMap => tMap.Node).Returns(_predicateObjectMap.R2RMLMappings.CreateUriNode(new Uri("http://tests.example.com/OtherTriplesMap")));

            // when 
            var objectMap1 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
            var objectMap2 = _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);

            // then
            Assert.NotSame(objectMap1, objectMap2);
            Assert.True(objectMap1 is RefObjectMapConfiguration);
            Assert.True(objectMap2 is RefObjectMapConfiguration);
            Assert.Equal(2, _predicateObjectMap.RefObjectMaps.Count());
        }

        [Fact]
        public void CanCreateObjectMapAndRefObjectMap()
        {
            // given
            Mock<ITriplesMapConfiguration> parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            parentTriplesMap.Setup(tMap => tMap.Node).Returns(_predicateObjectMap.R2RMLMappings.CreateUriNode(new Uri("http://tests.example.com/OtherTriplesMap")));

            // when
            _predicateObjectMap.CreateRefObjectMap(parentTriplesMap.Object);
            _predicateObjectMap.CreateObjectMap();

            // then
            Assert.Single(_predicateObjectMap.ObjectMaps);
            Assert.Single(_predicateObjectMap.RefObjectMaps);
        }
    }
}
