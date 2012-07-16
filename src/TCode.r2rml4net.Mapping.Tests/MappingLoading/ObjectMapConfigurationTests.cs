using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        [Test]
        public void CanBeInitializedWithExistingGraph()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap [ rr:template ""http://data.example.com/{JOB}"" ].");

            // when
            var objectMap = new ObjectMapConfiguration(graph.GetUriNode("ex:PredicateObjectMap"), graph);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual("http://data.example.com/{JOB}", objectMap.Template);
            Assert.AreEqual("http://www.example.com/PredicateObjectMap", ((IUriNode)objectMap.ParentMapNode).Uri.ToString());
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.ConfigurationNode);
        }

        [Test]
        public void CanBeInitializedWithConstantValue()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:objectMap [ rr:constant ex:someObject ].");

            // when
            var objectMap = new ObjectMapConfiguration(graph.GetUriNode("ex:PredicateObjectMap"), graph);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.ConfigurationNode);
        }

        [Test]
        public void CanBeInitializedWithConstantValueUsingShortcut()
        {
            // given
            IGraph graph = new Graph();
            graph.LoadFromString(
                @"@prefix ex: <http://www.example.com/>.
@prefix rr: <http://www.w3.org/ns/r2rml#>.

ex:triplesMap rr:predicateObjectMap ex:PredicateObjectMap .
  
ex:PredicateObjectMap rr:object ex:someObject .");

            // when
            var objectMap = new ObjectMapConfiguration(graph.GetUriNode("ex:PredicateObjectMap"), graph);
            objectMap.RecursiveInitializeSubMapsFromCurrentGraph(graph.GetBlankNode("autos1"));

            // then
            Assert.AreEqual(graph.CreateUriNode("ex:someObject").Uri, objectMap.ConstantValue);
            Assert.AreEqual(graph.GetBlankNode("autos1"), objectMap.ConfigurationNode);
        }
    }
}