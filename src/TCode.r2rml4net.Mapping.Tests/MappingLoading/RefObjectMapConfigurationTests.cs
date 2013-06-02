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
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;
        private Mock<ITriplesMapConfiguration> _referencedTriplesMap;
        private Mock<IPredicateObjectMap> _predicateObjectMap;

        [SetUp]
        public void Setup()
        {
            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap = new Mock<ITriplesMapConfiguration>();
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
        }

        [Test]
        public void CanInitializeJoinConditions()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(@"@prefix ex: <http://www.example.com/>.
                                   @prefix rr: <http://www.w3.org/ns/r2rml#>.

                                   ex:TriplesMap rr:predicateObjectMap [
                                       rr:predicate ex:department;
                                       rr:objectMap [
                                           rr:parentTriplesMap ex:TriplesMap2;
                                           rr:joinCondition [
                                               rr:child ""DEPTNO"";
                                               rr:parent ""ID"";
                                           ];
                                       ];
                                   ].");
            var predicateObjectMapNode = graph.GetTriplesWithPredicate(graph.CreateUriNode("rr:predicateObjectMap")).Single().Object;
            _predicateObjectMap.Setup(map => map.Node).Returns(predicateObjectMapNode);
            _parentTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap"));
            _referencedTriplesMap.Setup(tm => tm.Node).Returns(graph.GetUriNode("ex:TriplesMap2"));

            // when
            var blankNode = graph.GetTriplesWithPredicate(graph.CreateUriNode("rr:objectMap")).Single().Object;
            _refObjectMap = new RefObjectMapConfiguration(_predicateObjectMap.Object, _parentTriplesMap.Object, _referencedTriplesMap.Object, graph, blankNode, new MappingOptions());
            _refObjectMap.RecursiveInitializeSubMapsFromCurrentGraph();

            // then
            Assert.AreEqual(1, _refObjectMap.JoinConditions.Count());
            Assert.AreEqual("DEPTNO", _refObjectMap.JoinConditions.ElementAt(0).ChildColumn);
            Assert.AreEqual("ID", _refObjectMap.JoinConditions.ElementAt(0).ParentColumn);
            Assert.AreEqual(blankNode, _refObjectMap.Node);
        }
    }
}
