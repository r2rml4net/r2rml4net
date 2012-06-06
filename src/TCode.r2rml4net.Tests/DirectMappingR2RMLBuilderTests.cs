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
        private Mock<IDatabaseMetadataProvider> _databaseMetedata;

        [SetUp]
        public void Setup()
        {
            _databaseMetedata = new Mock<IDatabaseMetadataProvider>();
            _directMappingR2RMLBuilder = new DirectMappingR2RMLBuilder(_databaseMetedata.Object);
        }

        [Test, Description("First access to DirectMappingR2RMLBuilder#R2RMLGraph should execute reading of metadata")]
        public void AccesingGraphReadsDatabaseMetadata()
        {
            // when
            var graph = _directMappingR2RMLBuilder.R2RMLGraph;

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
        }

        [Test, Description("Metadata should be read from db only once")]
        public void AccesingGraphTwiceReadsDatabaseMetadataOnlyOnce()
        {
            // when
            var graph = _directMappingR2RMLBuilder.R2RMLGraph;
            graph = _directMappingR2RMLBuilder.R2RMLGraph;

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
        }
    }
}
