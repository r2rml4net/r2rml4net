using System;
using System.Collections.Generic;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class DefaultMappingStrategyTests
    {
        DefaultSubjectMapping _strategy;

        [TestCase(0, "_", null, ExpectedException = typeof(InvalidTriplesMapException))]
        [TestCase(3, "", "Table{\"Column1\"}{\"Column2\"}{\"Column3\"}")]
        [TestCase(3, "_", "Table_{\"Column1\"}_{\"Column2\"}_{\"Column3\"}")]
        [TestCase(3, ":", "Table:{\"Column1\"}:{\"Column2\"}:{\"Column3\"}")]
        [TestCase(8, ":", "Table:{\"Column1\"}:{\"Column2\"}:{\"Column3\"}:{\"Column4\"}:{\"Column5\"}:{\"Column6\"}:{\"Column7\"}:{\"Column8\"}")]
        public void GeneratesSubjectBlankNodesComposedOfAllColumns(int columnsCount, string columnSeparator, string expectedTemplate)
        {
            // given
            var columns = new List<string>();
            for (int i = 1; i <= columnsCount; i++)
            {
                columns.Add("Column" + i);
            }
            _strategy = new DefaultSubjectMapping(new DirectMappingOptions
                {
                    TemplateSeparator = columnSeparator
                });

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey("Table", columns);

            // then
            Assert.AreEqual(expectedTemplate, template);
        }

        [Test]
        public void CanGenerateTemplatesWithRegularIdentifiers()
        {
            // given
            var columns = new[] { "ColumnA", "Column B", "Yet another column" };
            _strategy = new DefaultSubjectMapping(new DirectMappingOptions
            {
                UseDelimitedIdentifiers = false
            });

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey("Table", columns);

            // then
            Assert.AreEqual("Table_{ColumnA}_{Column B}_{Yet another column}", template);
        }

        [TestCase("http://www.example.com/", new[] { "Id" }, "http://www.example.com/Table/Id={\"Id\"}")]
        [TestCase("http://www.example.com/", new[] { "PK 1", "PK2" }, "http://www.example.com/Table/PK%201={\"PK 1\"};PK2={\"PK2\"}")]
        public void GeneratesSubjectTemplateFromPrimaryKey(string baseUriString, string[] columns, string expected)
        {
            // given
            _strategy = new DefaultSubjectMapping(new DirectMappingOptions());

            // when
            var template = _strategy.CreateSubjectTemplateForPrimaryKey(new Uri(baseUriString), "Table", columns);

            // then
            Assert.AreEqual(expected, template);
        }
    }
}