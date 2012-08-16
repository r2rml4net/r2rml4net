using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class PrimaryKeyMappingStrategyTests
    {
        PrimaryKeyMappingStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new PrimaryKeyMappingStrategy(new MappingOptions());
        }

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
            _strategy = new PrimaryKeyMappingStrategy(new MappingOptions
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
            _strategy = new PrimaryKeyMappingStrategy(new MappingOptions
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
            var table = new TableMetadata { Name = "Table" };
            table.Add(new ColumnMetadata { Name = "Not primary key" });

            // when
            Assert.Throws<ArgumentException>(() => _strategy.CreateSubjectTemplateForPrimaryKey(new Uri("http://www.example.com/"), table));
        }

        [Test]
        public void LogsErrorOnGeneratingTemplateForMultipleReferencedUniqueKeysAndUsesTheShortest()
        {
            // given
            var table = RelationalTestMappings.NoPrimaryKeyThreeUniqueKeys["Student"];
            Mock<IDefaultMappingGenerationLog> log = new Mock<IDefaultMappingGenerationLog>();
            _strategy.Log = log.Object;

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey(table);

            // then
            Assert.AreEqual("Student_{\"ID\"}", template);
            log.Verify(l => l.LogMultipleCompositeKeyReferences(table), Times.Once());
        }

        [Test]
        public void WhenAUniqueKeyIsReferencedGeneratesBlankNodeForItsColumns()
        {
            // given
            var columnId = new ColumnMetadata
                {
                    Name = "ID",
                    Type = R2RMLType.Integer
                };
            var columnName = new ColumnMetadata
                {
                    Name = "PESEL",
                    Type = R2RMLType.String
                };
            var columnLastName = new ColumnMetadata
                {
                    Name = "LastName",
                    Type = R2RMLType.String
                };
            var studentsTable = new TableMetadata
                            {
                                columnId,
                                columnName,
                                columnLastName
                            };
            studentsTable.Name = "Student";
            studentsTable.UniqueKeys.Add(new UniqueKeyMetadata { columnId });
            var uniqueKey = new UniqueKeyMetadata { columnName };
            uniqueKey.IsReferenced = true;
            studentsTable.UniqueKeys.Add(uniqueKey);

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey(studentsTable);

            // then
            Assert.AreEqual("Student_{\"PESEL\"}", template);
        }

        [Test]
        public void WhenNoUniqueKeyIsReferencedGeneratesTemplateOfTheShortest()
        {
            // given
            var columnId = new ColumnMetadata
            {
                Name = "ID",
                Type = R2RMLType.Integer
            };
            var columnPesel = new ColumnMetadata
            {
                Name = "PESEL",
                Type = R2RMLType.String
            };
            var columnLastName = new ColumnMetadata
            {
                Name = "Name",
                Type = R2RMLType.String
            };
            var columnName = new ColumnMetadata
            {
                Name = "LastName",
                Type = R2RMLType.String
            };
            var yetAnotherColumn = new ColumnMetadata
            {
                Name = "another",
                Type = R2RMLType.String
            };
            var otherColumn = new ColumnMetadata
            {
                Name = "other",
                Type = R2RMLType.String
            };
            var studentsTable = new TableMetadata
                            {
                                columnId,
                                columnName,
                                columnLastName,
                                columnPesel,
                                otherColumn,
                                yetAnotherColumn
                            };
            studentsTable.Name = "Student";
            studentsTable.UniqueKeys.Add(new UniqueKeyMetadata { columnId });
            studentsTable.UniqueKeys.Add(new UniqueKeyMetadata { columnName, columnLastName });
            studentsTable.UniqueKeys.Add(new UniqueKeyMetadata { columnPesel, otherColumn, yetAnotherColumn });

            // when
            var template = _strategy.CreateSubjectTemplateForNoPrimaryKey(studentsTable);

            // then
            Assert.AreEqual("Student_{\"ID\"}", template);
        }

        [Test]
        public void GeneratesTemplateWithUnicodeCharacters()
        {
            // when
            var template = _strategy.CreateSubjectTemplateForPrimaryKey(new Uri("http://example.com/"), RelationalTestMappings.D017_I18NnoSpecialChars["植物"]);

            // then
            Assert.AreEqual("http://example.com/植物/名={\"名\"};使用部={\"使用部\"}", template);
        }
    }
}