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
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;
        private Mock<ITriplesMapConfiguration> _referencedTriplesMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new FluentR2RML().R2RMLMappings;
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            predicateObjectMap.Setup(map => map.Node).Returns(
                graph.CreateUriNode(new Uri("http://test.example.com/PredicateObjectMap")));

            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _parentTriplesMap.Setup(map => map.Node).Returns(graph.CreateUriNode(new Uri("http://test.example.com/TriplesMap")));
            _referencedTriplesMap = new Mock<ITriplesMapConfiguration>();
            _referencedTriplesMap.Setup(tm => tm.Node).Returns(graph.CreateUriNode(new Uri("http://test.example.com/OtherTriplesMap")));

            _refObjectMap = new RefObjectMapConfiguration(predicateObjectMap.Object, _parentTriplesMap.Object, _referencedTriplesMap.Object, graph);
        }

        [Test]
        public void CreatingAssertsRequiredTriples()
        {
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankObject("http://test.example.com/PredicateObjectMap", UriConstants.RrObjectMapProperty);
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrParentTriplesMapProperty, "http://test.example.com/TriplesMap");
        }

        [Test]
        public void CanCreateJoinConditions()
        {
            // given
            const string childColumn = "child";
            const string parentColumn = "parent";

            // when
            _refObjectMap.AddJoinCondition(childColumn + "1", parentColumn + "1");
            _refObjectMap.AddJoinCondition(childColumn + "2", parentColumn + "2");

            // then
            Assert.AreEqual(2, _refObjectMap.JoinConditions.Count());
            Assert.IsNotNull(_refObjectMap.JoinConditions.SingleOrDefault(jc => jc.ChildColumn == "child1" && jc.ParentColumn == "parent1"));
            Assert.IsNotNull(_refObjectMap.JoinConditions.SingleOrDefault(jc => jc.ChildColumn == "child2" && jc.ParentColumn == "parent2"));
        }

        [Test]
        public void UsesEffectiveSqlBuilder()
        {
            // given
            Mock<IR2RMLConfiguration> r2RML = new Mock<IR2RMLConfiguration>();
            Mock<ISqlQueryBuilder> sqlBuilder = new Mock<ISqlQueryBuilder>();
            const string excpetedSql = "SELECT * FROM (SELECT * FROM A) AS tmp";
            sqlBuilder.Setup(builder => builder.GetEffectiveQueryForRefObjectMap(It.IsAny<IRefObjectMap>()))
                      .Returns(excpetedSql);
            r2RML.Setup(config => config.SqlQueryBuilder).Returns(sqlBuilder.Object);
            _parentTriplesMap.Setup(tm => tm.R2RMLConfiguration).Returns(r2RML.Object);

            // when
            string sql = _refObjectMap.EffectiveSqlQuery;

            // then
            Assert.AreEqual(excpetedSql, sql);
            sqlBuilder.Verify(builder => builder.GetEffectiveQueryForRefObjectMap(It.IsAny<IRefObjectMap>()),
                              Times.Once());
            r2RML.Verify(config => config.SqlQueryBuilder, Times.Once());
            _parentTriplesMap.Verify(tm => tm.R2RMLConfiguration, Times.Once());
        }
    }
}
