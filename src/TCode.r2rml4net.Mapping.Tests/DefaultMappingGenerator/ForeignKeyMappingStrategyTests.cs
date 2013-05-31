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
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class ForeignKeyMappingStrategyTests
    {
        ForeignKeyMappingStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new ForeignKeyMappingStrategy(new MappingOptions());
        }

        [Test]
        public void GeneratesReferenceProperty()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK" },
                ReferencedColumns = new[] { "ID" }
            };

            // when
            var uri = _strategy.CreateReferencePredicateUri(new Uri("http://example.com"), foreignKey);

            // then
            Assert.AreEqual("http://example.com/Table#ref-FK", uri.AbsoluteUri);
        }

        [Test]
        public void GeneratesReferencePropertyForMulticolumnKey()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" }
            };

            // when
            var uri = _strategy.CreateReferencePredicateUri(new Uri("http://example.com"), foreignKey);

            // then
            Assert.AreEqual("http://example.com/Table#ref-FK1;FK2;FK3", uri.AbsoluteUri);
        }

        [Test]
        public void GeneratesReferenceObjectTemplate()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK" },
                ReferencedColumns = new[] { "ID" }
            };

            // when
            var template = _strategy.CreateReferenceObjectTemplate(new Uri("http://example.com"), foreignKey);

            // then

            Assert.AreEqual("http://example.com/Other/ID={\"FK\"}", template);
        }

        [Test]
        public void GeneratesReferenceObjectTemplateForMulticolumnKey()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" }
            };

            // when
            var template = _strategy.CreateReferenceObjectTemplate(new Uri("http://example.com"), foreignKey);

            // then
            Assert.AreEqual("http://example.com/Other/ID1={\"FK1\"};ID2={\"FK2\"};ID3={\"FK3\"}", template);
        }

        [Test]
        public void GeneratedReferencedObjectTemplateForCandidateKeyReference()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" },
                IsCandidateKeyReference = true
            };

            // when
            var template = _strategy.CreateObjectTemplateForCandidateKeyReference(foreignKey);

            // then
            Assert.AreEqual("Other_{\"FK1\"}_{\"FK2\"}_{\"FK3\"}", template);
        }

        [Test]
        public void ThrowsIfGeneratingForPrimaryKeyButIsCandidateKeyReference()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" },
                IsCandidateKeyReference = true
            };

            // when
            Assert.Throws<ArgumentException>(() => _strategy.CreateReferenceObjectTemplate(new Uri("http://example.com"), foreignKey));
        }

        [Test]
        public void ThrowsIfGeneratingForIsCandidateKeyButIsPrimaryKeyReference()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = new TableMetadata { Name = "Other" },
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" }
            };

            // when
            Assert.Throws<ArgumentException>(() => _strategy.CreateObjectTemplateForCandidateKeyReference(foreignKey));
        }

        [Test]
        public void GeneratesForeignKeyObjectTemplateForCandidateKeyRefWhereTableHasPrimaryKey()
        {
            // given
            var referencedTable = new TableMetadata { Name = "Other" };
            referencedTable.Add(new ColumnMetadata { IsPrimaryKey = true, Name = "ID" });
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTable = referencedTable,
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK" },
                ReferencedColumns = new[] { "ID" },
                IsCandidateKeyReference = false,
                ReferencedTableHasPrimaryKey = true
            };

            // when
            var template = _strategy.CreateReferenceObjectTemplate(new Uri("http://example.com/base/"), foreignKey);

            // then
            Assert.AreEqual("http://example.com/base/Other/ID={\"FK\"}", template);
        }

        [Test]
        public void GeneratesTemplateWithUnicodeCharacters()
        {
            // when
            var template = _strategy.CreateReferenceObjectTemplate(new Uri("http://example.com/"), RelationalTestMappings.D017_I18NnoSpecialChars["成分"].ForeignKeys[0]);

            // then
            Assert.AreEqual("http://example.com/植物/名={\"植物名\"};使用部={\"使用部\"}", template);
        }
    }
}