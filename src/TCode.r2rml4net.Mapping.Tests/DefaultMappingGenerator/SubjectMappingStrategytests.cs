using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class SubjectMappingStrategyTests
    {
        private PrimaryKeyMappingStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new PrimaryKeyMappingStrategy(new MappingOptions());
        }

        [TestCase("http://example.com")]
        [TestCase("http://example.com/")]
        public void CreatesSubjectClassUri(string baseUri)
        {
            // given
            TableMetadata table = new TableMetadata { Name = "TableXYZ" };

            // when
            var classUri = _strategy.CreateSubjectClassUri(new Uri(baseUri), table.Name);

            // then
            Assert.AreEqual("http://example.com/TableXYZ", classUri.AbsoluteUri);
        }
    }
}