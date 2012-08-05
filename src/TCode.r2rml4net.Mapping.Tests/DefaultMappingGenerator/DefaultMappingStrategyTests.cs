using System;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class DefaultMappingStrategyTests
    {
        PrimaryKeyMappingStrategy _strategy;

        [TestCase(0, "_", null, ExpectedException = typeof(InvalidTriplesMapException))]
        [TestCase(3, "", "Table{\"Column1\"}{\"Column2\"}{\"Column3\"}")]
        [TestCase(3, "_", "Table_{\"Column1\"}_{\"Column2\"}_{\"Column3\"}")]
        [TestCase(3, ":", "Table:{\"Column1\"}:{\"Column2\"}:{\"Column3\"}")]
        [TestCase(8, ":", "Table:{\"Column1\"}:{\"Column2\"}:{\"Column3\"}:{\"Column4\"}:{\"Column5\"}:{\"Column6\"}:{\"Column7\"}:{\"Column8\"}")]
        public void GeneratesSubjectBlankNodesComposedOfAllColumns(int columnsCount, string columnSeparator, string expectedTemplate)
        {
            // given
            TableMetadata table = new TableMetadata { Name = "Table" };
            for (int i = 1; i <= columnsCount; i++)
            {
                table.Add(new ColumnMetadata { Name = "Column" + i });
            }
            _strategy = new PrimaryKeyMappingStrategy(new DirectMappingOptions
                {
                    TemplateSeparator = columnSeparator
                });

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey(table);

            // then
            Assert.AreEqual(expectedTemplate, template);
        }

        [Test]
        public void CanGenerateTemplatesWithRegularIdentifiers()
        {
            // given
            TableMetadata table = new TableMetadata { Name = "Table" };
            foreach (var column in new[] { "ColumnA", "Column B", "Yet another column" })
            {
                table.Add(new ColumnMetadata { Name = column });
            }
            _strategy = new PrimaryKeyMappingStrategy(new DirectMappingOptions
            {
                UseDelimitedIdentifiers = false
            });

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey(table);

            // then
            Assert.AreEqual("Table_{ColumnA}_{Column B}_{Yet another column}", template);
        }

        [TestCase("http://www.example.com/", new[] { "Id" }, "http://www.example.com/Table/Id={\"Id\"}")]
        [TestCase("http://www.example.com/", new[] { "PK 1", "PK2" }, "http://www.example.com/Table/PK%201={\"PK 1\"};PK2={\"PK2\"}")]
        public void GeneratesSubjectTemplateFromPrimaryKey(string baseUriString, string[] columns, string expected)
        {
            // given
            _strategy = new PrimaryKeyMappingStrategy(new DirectMappingOptions());
            var table = new TableMetadata { Name = "Table" };
            foreach (var column in columns)
            {
                table.Add(new ColumnMetadata { Name = column, IsPrimaryKey = true });
            }

            // when
            var template = _strategy.CreateSubjectTemplateForPrimaryKey(new Uri(baseUriString), table);

            // then
            Assert.AreEqual(expected, template);
        }

        [Test]
        public void ThrowsOnTemplateGenerationIfNoPrimaryKey()
        {
            // given
            _strategy = new PrimaryKeyMappingStrategy(new DirectMappingOptions());
            var table = new TableMetadata { Name = "Table" };
            table.Add(new ColumnMetadata { Name = "Not primary key" });

            // when
            Assert.Throws<ArgumentException>(() => _strategy.CreateSubjectTemplateForPrimaryKey(new Uri("http://www.example.com/"), table));
        }
    }
}