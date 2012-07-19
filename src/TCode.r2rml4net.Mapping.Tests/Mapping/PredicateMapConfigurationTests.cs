using System;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class PredicateMapConfigurationTests
    {
        private PredicateMapConfiguration _predicateMap;
        private Mock<ITriplesMapConfiguration> _triplesMapNode;
       
        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            predicateObjectMap.Setup(map => map.Node).Returns(graph.CreateBlankNode("predicateObjectMap"));

            _triplesMapNode = new Mock<ITriplesMapConfiguration>();
            _triplesMapNode.Setup(tm => tm.Node).Returns(triplesMapNode);

            _predicateMap = new PredicateMapConfiguration(_triplesMapNode.Object, predicateObjectMap.Object, graph);
        }

        [Test]
        public void PredicateMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _predicateMap.IsConstantValued(uri);

            // then
            Assert.IsTrue(_predicateMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _predicateMap.ParentMapNode,
                    _predicateMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrPredicateMapProperty)),
                    _predicateMap.Node)));
            Assert.IsTrue(_predicateMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _predicateMap.Node,
                    _predicateMap.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _predicateMap.R2RMLMappings.CreateUriNode(uri))));
            Assert.AreEqual(uri, _predicateMap.ConstantValue);
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void PredicateMapCannotBeOfTypeLiteral()
        {
            _predicateMap.TermType.IsLiteral();
        }

        [Test, ExpectedException(typeof(InvalidTriplesMapException))]
        public void PredicateMapCannotBeOfTypeBlankNode()
        {
            _predicateMap.TermType.IsBlankNode();
        }

        [Test]
        public void PredicateIsNullByDefault()
        {
            Assert.IsNull(_predicateMap.URI);
        }

        [Test]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.AreEqual(new Uri("http://www.w3.org/ns/r2rml#predicate"), _predicateMap.CreateShortcutPropertyNode().Uri);
        }
    }
}