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

        private void TestMappingGeneration(TableCollection tables, string embeddedResourceGraph)
        {
            // given;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when 
            _directMappingR2RMLBuilder.BuildGraph();

            // then
            Graph expected = new Graph();
            expected.LoadFromEmbeddedResource(string.Format("TCode.r2rml4net.Tests.TestGraphs.{0}, TCode.r2rml4net.Tests", embeddedResourceGraph));

            Assert.IsTrue(_directMappingR2RMLBuilder.R2RMLGraph.Equals(expected));
        }

        [Test]
        public void DirectGraphTC0001_MappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.D001_1table1column, "R2RMLTC0001.ttl");
        }

        [Test]
        public void DirectGraphTC0002_MappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.D002_1table2columns, "R2RMLTC0002.ttl");
        }

        [Test]
        public void DirectGraphTC0003_MappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.D003_1table3columns, "R2RMLTC0003.ttl");
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
