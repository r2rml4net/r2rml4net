using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private GraphMapConfiguration _graphMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _graphMap = new GraphMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void GraphMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _graphMap.IsConstantValued(uri);

            // then
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.ParentMapNode,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrGraphProperty)),
                    _graphMap.R2RMLMappings.CreateUriNode(uri))));
        }
    }
}