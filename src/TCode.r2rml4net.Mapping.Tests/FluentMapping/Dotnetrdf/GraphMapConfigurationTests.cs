using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Dotnetrdf;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.FluentMapping.Dotnetrdf
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
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrGraphMapProperty)),
                    _graphMap.TermMapNode)));
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.TermMapNode,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _graphMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _graphMap.Graph);
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeLiteral()
        {
            _graphMap.TermType.IsLiteral();
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeBlankNode()
        {
            _graphMap.TermType.IsBlankNode();
        }

        [Test]
        public void GraphUriIsNullByDefault()
        {
            Assert.IsNull(_graphMap.Graph);
        }
    }
}