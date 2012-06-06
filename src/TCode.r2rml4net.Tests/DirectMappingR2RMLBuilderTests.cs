using NUnit.Framework;
using Moq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class DirectMappingR2RMLBuilderTests
    {
        private DirectMappingR2RMLBuilder _directMappingR2RMLBuilder;
        private Mock<IDatabaseMetadata> _databaseMetedata;
        private Mock<IDatabaseMetadataVisitor> _databaseMetedataVisitor;

        [SetUp]
        public void Setup()
        {
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _databaseMetedataVisitor = new Mock<IDatabaseMetadataVisitor>();
            _directMappingR2RMLBuilder = new DirectMappingR2RMLBuilder(_databaseMetedata.Object);
        }

        [Test, Description("First access to DirectMappingR2RMLBuilder#R2RMLGraph should execute reading of metadata")]
        public void AccesingGraphReadsDatabaseMetadata()
        {
            // when
            var graph = _directMappingR2RMLBuilder.R2RMLGraph;

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
            Assert.IsNotNull(graph);
        }

        [Test, Description("Metadata should be read from db only once")]
        public void AccesingGraphTwiceReadsDatabaseMetadataOnlyOnce()
        {
            // when
            var graph = _directMappingR2RMLBuilder.R2RMLGraph;
            graph = _directMappingR2RMLBuilder.R2RMLGraph;

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
            Assert.IsNotNull(graph);
        }

        [Test]
        public void BuildingGraphReadsTablesCollection()
        {
            // given
            var tables = new TableCollection();
            _databaseMetedata.Setup(db => db.Tables).Returns(tables);

            // when
            var graph = _directMappingR2RMLBuilder.R2RMLGraph;

            // then
            _databaseMetedata.Verify(db => db.Tables, Times.Once());
            _databaseMetedataVisitor.Verify(visitor => visitor.Visit(tables), Times.Once());
            Assert.IsNotNull(graph);
        }
    }
}
