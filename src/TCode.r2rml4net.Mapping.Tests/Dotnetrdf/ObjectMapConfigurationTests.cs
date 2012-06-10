using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class ObjectMapConfigurationTests
    {
        private ObjectMapConfiguration _objectMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode triplesMapNode = graph.CreateUriNode(new Uri("http://test.example.com/TestMapping"));
            _objectMap = new ObjectMapConfiguration(triplesMapNode, graph);
        }

        [Test]
        public void ObjectMapCanBeLiteralConstantValued()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            _objectMap.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(UriConstants.RrObjectProperty, literal);
        }

        [Test]
        public void ConstantValueCanBeSetOnlyOnce()
        {
            // given
            const string literal = "Some text";

            // when
            _objectMap.IsConstantValued(literal);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _objectMap.IsConstantValued(literal));
        }

        [Test]
        public void ObjectMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _objectMap.IsConstantValued(uri);

            // then
            _objectMap.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrObjectProperty, uri);
        }
    }
}