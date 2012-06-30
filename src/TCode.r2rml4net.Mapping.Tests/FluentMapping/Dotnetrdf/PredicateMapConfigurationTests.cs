using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Dotnetrdf;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.FluentMapping.Dotnetrdf
{
    [TestFixture]
    public class PredicateMapConfigurationTests
    {
        private PredicateMapConfiguration _predicateMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _predicateMap = new PredicateMapConfiguration(triplesMapNode, graph);
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
                    _predicateMap.TermMapNode)));
            Assert.IsTrue(_predicateMap.R2RMLMappings.ContainsTriple(
                new Triple(
                    _predicateMap.TermMapNode,
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
            Assert.IsNull(_predicateMap.Predicate);
        }
    }
}