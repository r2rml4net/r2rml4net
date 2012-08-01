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
            // given
            _mappingStrategy.Setup(ms => ms.CreateUriForTable(_mappingBaseUri, "Student")).Returns(new Uri("http://template.uri"));
            _mappingStrategy.Setup(ms => ms.CreateTemplateForNoPrimaryKey("Student", It.IsAny<IEnumerable<string>>())).Returns("template");
            _databaseMetedata.Setup(meta => meta.Tables).Returns(RelationalTestMappings.D001_1table1column);

            // when
            _generator.GenerateMappings();

            // then
            _mappingStrategy.Verify(ms => ms.CreateUriForTable(_mappingBaseUri, "Student"), Times.Once());
            _mappingStrategy.Verify(ms => ms.CreateTemplateForNoPrimaryKey("Student", It.Is<IEnumerable<string>>(c => c.Contains("Name"))), Times.Once());
        }

        [Test]
        public void TestForTableWithMultipleColumns()
        {
            // given
            _mappingStrategy.Setup(ms => ms.CreateUriForTable(_mappingBaseUri, "Student")).Returns(new Uri("http://template.uri"));
            _mappingStrategy.Setup(ms => ms.CreateTemplateForNoPrimaryKey("Student", It.IsAny<IEnumerable<string>>())).Returns("template");
            var tableCollection = RelationalTestMappings.TypedColumns;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tableCollection);

            // when
            _generator.GenerateMappings();

            // then
            _mappingStrategy.Verify(ms => ms.CreateUriForTable(_mappingBaseUri, "Student"), Times.Once());
            _mappingStrategy.Verify(ms => ms.CreateTemplateForNoPrimaryKey("Student", It.Is<IEnumerable<string>>(names => names.Count()==15)), Times.Once());
        }
    }
}