#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping.Direct;
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
            _strategy = new PrimaryKeyMappingStrategy();
        }

        [TestCase(0, "_", null, ExpectedException = typeof(InvalidMapException))]
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
            _strategy = new PrimaryKeyMappingStrategy();

            // when
            string template;
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator(columnSeparator)))
            {
                template = _strategy.CreateSubjectTemplateForNoPrimaryKey(table);
            }

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
            _strategy = new PrimaryKeyMappingStrategy();

            // when
            string template;
            using (new MappingScope(new MappingOptions().UsingDelimitedIdentifiers(false)))
            {
                template = _strategy.CreateSubjectTemplateForNoPrimaryKey(table);
            }

            // then
            Assert.AreEqual("Table_{ColumnA}_{Column B}_{Yet another column}", template);
        }

        [TestCase("http://www.example.com/", new[] { "Id" }, "http://www.example.com/Table/Id={\"Id\"}")]
        [TestCase("http://www.example.com/", new[] { "PK 1", "PK2" }, "http://www.example.com/Table/PK%201={\"PK 1\"};PK2={\"PK2\"}")]
        public void GeneratesSubjectTemplateFromPrimaryKey(string BaseUriString, string[] columns, string expected)
        {
            // given
            var table = new TableMetadata { Name = "Table" };
            foreach (var column in columns)
            {
                table.Add(new ColumnMetadata { Name = column, IsPrimaryKey = true });
            }

            // when
            var template = _strategy.CreateSubjectTemplateForPrimaryKey(new Uri(BaseUriString), table);

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
            Mock<LogFacadeBase> log = new Mock<LogFacadeBase>();
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