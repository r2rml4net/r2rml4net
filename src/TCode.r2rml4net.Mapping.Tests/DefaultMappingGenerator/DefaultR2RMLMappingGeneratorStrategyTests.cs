using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.Mapping.Tests.Mocks;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class DefaultR2RMLMappingGeneratorStrategyTests
    {
        private R2RMLMappingGenerator _generator;
        private Mock<IDatabaseMetadata> _databaseMetedata;
        private Mock<IDirectMappingStrategy> _mappingStrategy;
        private Mock<IForeignKeyMappingStrategy> _foreignKeyStrategy;
        private Mock<IColumnMappingStrategy> _columnStrategy;
        private Mock<IR2RMLConfiguration> _configuration;
        private readonly Uri _mappingBaseUri = new Uri("http://base.uri/");

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IR2RMLConfiguration>();
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _mappingStrategy = new Mock<IDirectMappingStrategy>(MockBehavior.Strict);
            _foreignKeyStrategy = new Mock<IForeignKeyMappingStrategy>(MockBehavior.Strict);
            _columnStrategy = new Mock<IColumnMappingStrategy>(MockBehavior.Strict);
            _generator = new R2RMLMappingGenerator(_databaseMetedata.Object, _configuration.Object)
                {
                    MappingStrategy = _mappingStrategy.Object,
                    ForeignKeyMappingStrategy = _foreignKeyStrategy.Object,
                    ColumnMappingStrategy = _columnStrategy.Object,
                    MappingBaseUri = _mappingBaseUri
                };
            _configuration.Setup(conf => conf.CreateTriplesMapFromR2RMLView(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
            _configuration.Setup(conf => conf.CreateTriplesMapFromTable(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
        }

        [Test]
        public void TestForTableWithoutPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D003_1table3columns);
        }

        [Test]
        public void AnotherTestForTableWithoutPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D001_1table1column);
        }

        [Test]
        public void TestForTableWithMultipleColumns()
        {
            TestStrategyUsage(RelationalTestMappings.TypedColumns);
        }

        [Test]
        public void TestForTableWithCompositePrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D008_1table1compositeprimarykey3columns);
        }

        [Test]
        public void TestForTableWithPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D006_1table1primarykey1column);
        }

        [Test]
        public void TestForTableWithForeignKey()
        {
            TestStrategyUsage(RelationalTestMappings.D009_2tables1primarykey1foreignkey);
        }

        private void TestStrategyUsage(TableCollection tables)
        {
            // given
            _mappingStrategy.Setup(ms => ms.CreateSubjectMapForPrimaryKey(It.IsAny<ISubjectMapConfiguration>(), _mappingBaseUri, It.IsAny<TableMetadata>()));
            _mappingStrategy.Setup(ms => ms.CreateSubjectMapForNoPrimaryKey(It.IsAny<ISubjectMapConfiguration>(), _mappingBaseUri, It.IsAny<TableMetadata>()));
            _columnStrategy.Setup(ms => ms.CreatePredicateUri(_mappingBaseUri, It.IsAny<ColumnMetadata>()))
                           .Returns(new Uri("http://predicate.uri"));
            _foreignKeyStrategy.Setup(ms => ms.CreateReferencePredicateUri(_mappingBaseUri, It.IsAny<ForeignKeyMetadata>()))
                               .Returns(new Uri("http://ref.uri"));
            _foreignKeyStrategy.Setup(ms => ms.CreateReferenceObjectTemplate(_mappingBaseUri, It.IsAny<ForeignKeyMetadata>()))
                               .Returns("termplate");
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when
            _generator.GenerateMappings();

            // then
            foreach (var table in tables)
            {
                foreach (var column in table)
                {
                    ColumnMetadata column1 = column;
                    _columnStrategy.Verify(ms => ms.CreatePredicateUri(_mappingBaseUri, column1), Times.Once());
                }

                //if(table.ForeignKeys != null)
                foreach (var fk in table.ForeignKeys)
                {
                    ForeignKeyMetadata fk1 = fk;
                    _foreignKeyStrategy.Verify(ms =>ms.CreateReferencePredicateUri(_mappingBaseUri, fk1),Times.Once());
                    _foreignKeyStrategy.Verify(ms =>ms.CreateReferenceObjectTemplate(_mappingBaseUri,fk1));
                }
            }

            foreach (var table in tables.Where(t => t.PrimaryKey.Length == 0))
            {
                TableMetadata table1 = table;
                _mappingStrategy.Setup(ms =>
                    ms.CreateSubjectMapForNoPrimaryKey(
                    It.IsAny<ISubjectMapConfiguration>(),
                    _mappingBaseUri,
                    table1));

            }

            foreach (var table in tables.Where(t => t.PrimaryKey.Length > 0))
            {
                TableMetadata table1 = table;
                _mappingStrategy.Setup(ms =>
                    ms.CreateSubjectMapForPrimaryKey(
                    It.IsAny<ISubjectMapConfiguration>(),
                    _mappingBaseUri,
                    table1));
            }
        }
    }
}