using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private GraphMapConfiguration _objectMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _objectMap = new GraphMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void PropertyMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _objectMap.IsConstantValued(uri);

            // then
            _objectMap.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrGraphProperty, uri);
        }
    }
}