using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class ColumnMappingStrategyTests
    {
        readonly ColumnMappingStrategy _strategy = new ColumnMappingStrategy();

        [TestCase("http://example.com/")]
        [TestCase("http://example.com")]
        public void CreatesATemplateForColumn(string baseUri)
        {
            // given
            var columnMetadata = new ColumnMetadata
                {
                    Name = "ColumnXYZ",
                    Table = new TableMetadata
                        {
                            Name = "TableABC"
                        }
                };

            // when
            var uri = _strategy.CreatePredicateUri(new Uri(baseUri), columnMetadata);

            // then
            Assert.AreEqual("http://example.com/TableABC#ColumnXYZ", uri.ToString());
        }

        [Test]
        public void ThrowsIfAnyParamIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _strategy.CreatePredicateUri(new Uri("http://www.example.com"), null));
            Assert.Throws<ArgumentNullException>(() => _strategy.CreatePredicateUri(null, new ColumnMetadata()));
        }

        [Test]
        public void ThrowsIfColumnNameIsMissing()
        {
            Assert.Throws<ArgumentException>(() => _strategy.CreatePredicateUri(new Uri("http://www.example.com"), new ColumnMetadata()));
        }
    }
}