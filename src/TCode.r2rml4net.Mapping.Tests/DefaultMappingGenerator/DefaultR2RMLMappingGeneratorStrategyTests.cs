using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DefaultMapping;
using TCode.r2rml4net.Mapping.Tests.Mocks;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class DefaultR2RMLMappingGeneratorStrategyTests
    {
        private DefaultR2RMLMappingGenerator _generator;
        private Mock<IDatabaseMetadata> _databaseMetedata;
        private Mock<IDirectMappingStrategy> _mappingStrategy;
        private Mock<IR2RMLConfiguration> _configuration;
        private readonly Uri _mappingBaseUri = new Uri("http://base.uri/");
        private readonly Uri _mappedDataBaseUri = new Uri("http://data.uri/");

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IR2RMLConfiguration>();
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _mappingStrategy = new Mock<IDirectMappingStrategy>(MockBehavior.Strict);
            _generator = new DefaultR2RMLMappingGenerator(_databaseMetedata.Object, _configuration.Object)
                {
                    MappingStrategy = _mappingStrategy.Object,
                    MappingBaseUri = _mappingBaseUri,
                    MappedDataBaseUri = _mappedDataBaseUri
                };
            _configuration.Setup(conf => conf.CreateTriplesMapFromR2RMLView(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
            _configuration.Setup(conf => conf.CreateTriplesMapFromTable(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
        }

        [Test]
        public void TestForSimpleTable()
        {
            TestStrategyUsage(RelationalTestMappings.D001_1table1column);
        }

        [Test]
        public void TestForTableWithMultipleColumns()
        {
            TestStrategyUsage(RelationalTestMappings.TypedColumns);
        }

        private void TestStrategyUsage(TableCollection tables)
        {
            // given
            _mappingStrategy.Setup(ms => ms.CreateSubjectUri(_mappingBaseUri, It.IsAny<string>())).Returns(new Uri("http://template.uri"));
            _mappingStrategy.Setup(ms => ms.CreateSubjectTemplateForNoPrimaryKey(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("template");
            _mappingStrategy.Setup(ms => ms.CreatePredicateUri(_mappingBaseUri, It.IsAny<string>(), It.IsAny<string>())).Returns(new Uri("http://predicate.uri"));
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when
            _generator.GenerateMappings();

            // then
            foreach (var table in tables)
            {
                var tableName = table.Name;
                var columnsCount = table.ColumnsCount;
                _mappingStrategy.Verify(ms => ms.CreatePredicateUri(_mappingBaseUri, tableName, It.IsAny<string>()),
                                        Times.Exactly(columnsCount));
                _mappingStrategy.Verify(ms => ms.CreateSubjectUri(_mappingBaseUri, tableName), Times.Once());
            }

            foreach (var table in tables.Where(t => t.PrimaryKey.Length == 0))
            {
                var tableName = table.Name;
                var columnsCount = table.ColumnsCount;
                _mappingStrategy.Verify(
                    ms =>
                    ms.CreateSubjectTemplateForNoPrimaryKey(tableName, It.Is<IEnumerable<string>>(names => names.Count() == columnsCount)),
                    Times.Once());
            }
        }
    }
}