using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class SubjectMappingStrategyTests
    {
        private SubjectMappingStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new SubjectMappingStrategy(new DirectMappingOptions());
        }

        [TestCase("http://example.com")]
        [TestCase("http://example.com/")]
        public void CreatesSubjectUri(string baseUri)
        {
            // given
            TableMetadata table = new TableMetadata { Name = "TableXYZ" };

            // when
            var subjectUri = _strategy.CreateSubjectUri(new Uri(baseUri), table);

            // then
            Assert.AreEqual("http://example.com/TableXYZ", subjectUri.ToString());
        }
    }
}