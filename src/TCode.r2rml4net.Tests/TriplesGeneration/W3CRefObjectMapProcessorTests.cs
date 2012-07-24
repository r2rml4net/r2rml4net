using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    class W3CRefObjectMapProcessorTests : TriplesGenerationTestsBase
    {
        private W3CRefObjectMapProcessor _processor;
        private Mock<IRdfHandler> _rdfHandler;
        private Mock<ISubjectMap> _subjectMap;
        private Mock<IRefObjectMap> _refObjMap;
        private Mock<IDbConnection> _connection;
        private Mock<IRDFTermGenerator> _termGenerator;

        [SetUp]
        public void Setup()
        {
            _termGenerator = new Mock<IRDFTermGenerator>();
            _connection = new Mock<IDbConnection>();
            _refObjMap = new Mock<IRefObjectMap>();
            _rdfHandler = new Mock<IRdfHandler>();
            _subjectMap = new Mock<ISubjectMap>();

            _refObjMap.Setup(map => map.SubjectMap).Returns(_subjectMap.Object);

            _processor = new W3CRefObjectMapProcessor(_termGenerator.Object, _rdfHandler.Object);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void GeneratesSubjectForEachLogicalRow(int rowsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, new IGraphMap[0]);

            // then
            _termGenerator.Verify(gen=>gen.GenerateTerm<INode>(_subjectMap.Object, It.IsAny<IDataRecord>()), Times.Exactly(rowsCount));
        }
    }
}