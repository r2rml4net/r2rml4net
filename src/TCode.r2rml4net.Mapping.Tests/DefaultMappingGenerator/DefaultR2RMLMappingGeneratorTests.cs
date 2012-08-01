using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.DefaultMapping;
using TCode.r2rml4net.RDB;
using VDS.RDF;
using System.IO;
using VDS.RDF.Writing;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
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

        [Test]
        public void CreatedWithDefaultGenerationAlgorithm()
        {
            Assert.IsTrue(_defaultR2RMLMappingGenerator.MappingStrategy is DefaultMappingStrategy);
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
            _databaseMetedata.Verify(db => db.Tables, Times.Exactly(2));
            Assert.IsTrue(_configuration.GraphReadOnly.IsEmpty);
        }

        private void TestMappingGeneration(TableCollection tables, string embeddedResourceGraph)
        {
            // given;
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when 
            _defaultR2RMLMappingGenerator.GenerateMappings();

            // then
            Graph expected = new Graph();
            expected.LoadFromEmbeddedResource(string.Format("TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator.TestGraphs.{0}, TCode.r2rml4net.Mapping.Tests", embeddedResourceGraph));

            var serializedGraph = Serialize(_configuration.GraphReadOnly);
            var message = string.Format("Graphs aren't equal. Actual graph was:\r\n\r\n{0}", serializedGraph);

            var diff = expected.Difference(_configuration.GraphReadOnly);
            Assert.IsFalse(diff.AddedMSGs.Any() || diff.RemovedMSGs.Any() || diff.AddedTriples.Any() || diff.RemovedTriples.Any(), message);
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

        [Test]
        public void TableWithSpacesInNames()
        {
            TestMappingGeneration(RelationalTestMappings.D010_1table1primarykey3colums, "R2RMLTC0010.ttl");
        }

        [Test]
        public void TablesWithManyToManyRelations()
        {
            TestMappingGeneration(RelationalTestMappings.D011_M2MRelations, "R2RMLTC0011.ttl");
        }

        [Test]
        public void TablesWithReferenceToCandidateKey()
        {
            TestMappingGeneration(RelationalTestMappings.D014_3tables1primarykey1foreignkey, "R2RMLTC0014.ttl");
        }

        [Test]
        public void AnotherCompositeKeyCase()
        {
            TestMappingGeneration(RelationalTestMappings.D015_1table3columns1composityeprimarykey, "R2RMLTC0015.ttl");
        }

        [Test]
        public void TableWithManyDatatypes()
        {
            TestMappingGeneration(RelationalTestMappings.D016_1table1primarykey10columnsSQLdatatypes, "R2RMLTC0016.ttl");
        }

        [Test]
        public void InternationalizedTable()
        {
            TestMappingGeneration(RelationalTestMappings.D017_I18NnoSpecialChars, "R2RMLTC0017.ttl");
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
