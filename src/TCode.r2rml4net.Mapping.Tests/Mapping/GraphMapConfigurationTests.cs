using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class GraphMapConfigurationTests
    {
        private GraphMapConfiguration _graphMap;
        private Mock<ITriplesMapConfiguration> _triplesMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            Mock<IPredicateObjectMapConfiguration> predicateObjectMap = new Mock<IPredicateObjectMapConfiguration>();
            predicateObjectMap.Setup(map => map.Node).Returns(graph.CreateBlankNode("predicateObjectMap"));

            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(triplesMapNode);

            _graphMap = new GraphMapConfiguration(_triplesMap.Object, predicateObjectMap.Object, graph);
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
                    _graphMap.Node)));
            Assert.IsTrue(_graphMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _graphMap.Node,
                    _graphMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _graphMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _graphMap.URI);
        }

        [Test, ExpectedException(typeof (InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeLiteral()
        {
            _graphMap.TermType.IsLiteral();
        }

        [Test, ExpectedException(typeof (InvalidTriplesMapException))]
        public void GraphMapCannotBeOfTypeBlankNode()
        {
            _graphMap.TermType.IsBlankNode();
        }

        [Test]
        public void GraphUriIsNullByDefault()
        {
            Assert.IsNull(_graphMap.URI);
        }

        [Test]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.AreEqual(new Uri("http://www.w3.org/ns/r2rml#graph"), _graphMap.CreateShortcutPropertyNode().Uri);
        }
    }
}