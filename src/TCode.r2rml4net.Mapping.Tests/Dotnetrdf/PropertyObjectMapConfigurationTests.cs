using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class PropertyObjectMapConfigurationTests
    {
        private PropertyObjectMapConfiguration _propertyObjectMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            var triplesMapNode = graph.CreateUriNode(new Uri("http://tests.example.com/TriplesMap"));
            _propertyObjectMap = new PropertyObjectMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void CanCreateObjectMaps()
        {
            // when 
            var objectMap1 = _propertyObjectMap.CreateObjectMap();
            var objectMap2 = _propertyObjectMap.CreateObjectMap();

            // then
            Assert.AreNotSame(objectMap1, objectMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(objectMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(objectMap2);
        }

        [Test]
        public void CanCreatePredicateMaps()
        {
            // when 
            var propertyMap1 = _propertyObjectMap.CreatePropertyMap();
            var propertyMap2 = _propertyObjectMap.CreatePropertyMap();

            // then
            Assert.AreNotSame(propertyMap1, propertyMap2);
            Assert.IsInstanceOf<TermMapConfiguration>(propertyMap1);
            Assert.IsInstanceOf<TermMapConfiguration>(propertyMap2);
        }
    }
}
