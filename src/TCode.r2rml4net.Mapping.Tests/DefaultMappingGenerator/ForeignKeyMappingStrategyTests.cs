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
                ReferencedTableName = "Other",
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
                ReferencedTableName = "Other",
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
                ReferencedTableName = "Other",
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
                ReferencedTableName = "Other",
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
                ReferencedTableName = "Other",
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" },
                IsCandidateKeyReference = true
            };

            // when
            var template = _strategy.CreateObjectTemplateForCandidateKeyReference(foreignKey);

            // then
            Assert.AreEqual("Other_{\"ID1\"}_{\"ID2\"}_{\"ID3\"}", template);
        }

        [Test]
        public void ThrowsIfGeneratingForPrimaryKeyButIsCandidateKeyReference()
        {
            // given
            var foreignKey = new ForeignKeyMetadata
            {
                ReferencedTableName = "Other",
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
                ReferencedTableName = "Other",
                TableName = "Table",
                ForeignKeyColumns = new[] { "FK1", "FK2", "FK3" },
                ReferencedColumns = new[] { "ID1", "ID2", "ID3" }
            };

            // when
            Assert.Throws<ArgumentException>(() => _strategy.CreateObjectTemplateForCandidateKeyReference(foreignKey));
        }
    }
}