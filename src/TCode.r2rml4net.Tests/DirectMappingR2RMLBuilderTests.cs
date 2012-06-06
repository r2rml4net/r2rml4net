using NUnit.Framework;
using Moq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
using System.Data;
using VDS.RDF;
using System.IO;
using VDS.RDF.Writing;
using System.Collections.Generic;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class DirectMappingR2RMLBuilderTests
    {
        private DirectMappingR2RMLBuilder _directMappingR2RMLBuilder;
        private Mock<IDatabaseMetadata> _databaseMetedata;

        [SetUp]
        public void Setup()
        {
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _directMappingR2RMLBuilder = new DirectMappingR2RMLBuilder(_databaseMetedata.Object);
        }

        [Test, Description("Invoking DirectMappingR2RMLBuilder#BuildGraph should execute reading of metadata")]
        public void AccesingGraphReadsDatabaseMetadata()
        {
            // when
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
        }

        [Test, Description("Metadata should be read from db only once")]
        public void AccesingGraphTwiceReadsDatabaseMetadataOnlyOnce()
        {
            // when
            _directMappingR2RMLBuilder.BuildGraph();
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
        }

        [Test, Description("Building graph visits the table collection")]
        public void BuildingGraphReadsTablesCollection()
        {
            // given
            var tables = new TableCollection();
            _databaseMetedata.Setup(db => db.Tables).Returns(tables);

            // when
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            _databaseMetedata.Verify(db => db.Tables, Times.Once());
            Assert.IsNotNull(_directMappingR2RMLBuilder.R2RMLGraph);
        }

        [Test]
        public void DirectGraphTC0001_MappingGeneration()
        {
            // given
            var tables = RelationalTestMappings.D001_1table1column;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when 
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            Graph expected = new Graph();
            expected.LoadFromEmbeddedResource("TCode.r2rml4net.Tests.TestGraphs.R2RMLTC0001.ttl, TCode.r2rml4net.Tests");

            Assert.IsTrue(_directMappingR2RMLBuilder.R2RMLGraph.Equals(expected));
        }

        [Test]
        public void DirectGraphTC0002_MappingGeneration()
        {
            // given
            var tables = RelationalTestMappings.D002_1table2columns;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when 
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            Graph expected = new Graph();
            expected.LoadFromEmbeddedResource("TCode.r2rml4net.Tests.TestGraphs.R2RMLTC0002.ttl, TCode.r2rml4net.Tests");

            Assert.IsTrue(_directMappingR2RMLBuilder.R2RMLGraph.Equals(expected));
        }

        private string Serialize<TWriter>(IGraph graph) where TWriter : IRdfWriter, new()
        {
            using (TextWriter writer = new System.IO.StringWriter())
            {
                IRdfWriter turtle = new TWriter();
                turtle.Save(graph, writer);
                return writer.ToString();
            }
        }
    }
}
