using System;
using NUnit.Framework;
using Moq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using TCode.r2rml4net.RDB;
using VDS.RDF;
using System.IO;
using VDS.RDF.Writing;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class DefaultR2RMLMappingGeneratorTests
    {
        private DefaultR2RMLMappingGenerator _defaultR2RMLMappingGenerator;
        private Mock<IDatabaseMetadata> _databaseMetedata;
        private R2RMLConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _configuration = new R2RMLConfiguration(new Uri("http://mappingpedia.org/rdb2rdf/r2rml/tc/"));
            _defaultR2RMLMappingGenerator = new DefaultR2RMLMappingGenerator(_databaseMetedata.Object, _configuration);
        }

        [Test, Description("Invoking DefaultR2RMLMappingGenerator#GenerateMappings should execute reading of metadata")]
        public void AccesingGraphReadsDatabaseMetadata()
        {
            // when
            _defaultR2RMLMappingGenerator.GenerateMappings();

            // then
            _databaseMetedata.Verify(provider => provider.ReadMetadata(), Times.Once());
        }

        [Test, Description("Metadata should be read from db only once")]
        public void AccesingGraphTwiceReadsDatabaseMetadataOnlyOnce()
        {
            // when
            _defaultR2RMLMappingGenerator.GenerateMappings();
            _defaultR2RMLMappingGenerator.GenerateMappings();

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
            _defaultR2RMLMappingGenerator.GenerateMappings();

            // then
            _databaseMetedata.Verify(db => db.Tables, Times.Once());
            Assert.IsFalse(_configuration.GraphReadOnly.IsEmpty);
        }

        private void TestMappingGeneration(TableCollection tables, string embeddedResourceGraph)
        {
            // given;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when 
            _defaultR2RMLMappingGenerator.GenerateMappings();

            // then
            Graph expected = new Graph();
            expected.LoadFromEmbeddedResource(string.Format("TCode.r2rml4net.Tests.TestGraphs.{0}, TCode.r2rml4net.Tests", embeddedResourceGraph));

            var serializedGraph = Serialize(_configuration.GraphReadOnly);
            var message = string.Format("Graphs aren't equal. Actual graph was:\r\n\r\n{0}", serializedGraph);
            Assert.IsTrue(_configuration.GraphReadOnly.Equals(expected), message);
        }

        [Test]
        public void SimpleTableMappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.D001_1table1column, "R2RMLTC0001.ttl");
        }

        [Test]
        public void TypedColumnsMappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.TypedColumns, "R2RMLTC0002.ttl");
        }

        [Test]
        public void SimpleTableWith3ColumnsMappingGeneration()
        {
            TestMappingGeneration(RelationalTestMappings.D003_1table3columns, "R2RMLTC0003.ttl");
        }

        [Test]
        public void TableWithVarcharPrimaryKey()
        {
            TestMappingGeneration(RelationalTestMappings.D006_1table1primarykey1column, "R2RMLTC0006.ttl");
        }

        [Test]
        public void TableWithCompositePrimaryKey()
        {
            TestMappingGeneration(RelationalTestMappings.D008_1table1compositeprimarykey3columns, "R2RMLTC0008.ttl");
        }

        [Test]
        public void TwoTablesWithForeignKeyReference()
        {
            TestMappingGeneration(RelationalTestMappings.D009_2tables1primarykey1foreignkey, "R2RMLTC0009.ttl");
        }

        private string Serialize(IGraph graph) 
        {
            using (TextWriter writer = new System.IO.StringWriter())
            {
                var turtle = new CompressingTurtleWriter(10);
                turtle.Save(graph, writer);
                return writer.ToString();
            }
        }
    }
}
