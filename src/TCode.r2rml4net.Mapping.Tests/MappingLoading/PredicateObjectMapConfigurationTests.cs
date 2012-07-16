using System;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using Moq;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class PredicateObjectMapConfigurationTests
    {
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

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(graph.GetUriNode("ex:triplesMap"), graph);
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetUriNode("ex:PredicateObjectMap"));

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual(1, predicateObjectMap.PredicateMaps.Count(pm => pm.Template.Equals("http://data.example.com/employee/{EMPNO}")));
            Assert.AreEqual(1, predicateObjectMap.PredicateMaps.Count(pm => pm.Template.Equals("http://data.example.com/user/{EMPNO}")));
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

            // when
            var predicateObjectMap = new PredicateObjectMapConfiguration(graph.GetUriNode("ex:triplesMap"), graph);
            predicateObjectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetUriNode("ex:PredicateObjectMap"));

            // then
            Assert.AreEqual(2, predicateObjectMap.PredicateMaps.Count());
            Assert.AreEqual(1, predicateObjectMap.PredicateMaps.Count(pm => new Uri("http://www.example.com/Employee").Equals(pm.Predicate)));
            Assert.AreEqual(1, predicateObjectMap.PredicateMaps.Count(pm => new Uri("http://www.example.com/Worker").Equals(pm.Predicate)));
        }
    }
}
