using System;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
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
        private Mock<IPredicateObjectMap> _predObjectMap;
        private Mock<ITriplesGenerationLog> _log;

        [SetUp]
        public void Setup()
        {
            _termGenerator = new Mock<IRDFTermGenerator>();
            _connection = new Mock<IDbConnection>();
            _refObjMap = new Mock<IRefObjectMap>();
            _rdfHandler = new Mock<IRdfHandler>();
            _subjectMap = new Mock<ISubjectMap>();
            _predObjectMap = new Mock<IPredicateObjectMap>();
            _log = new Mock<ITriplesGenerationLog>();

            _rdfHandler.Setup(handler => handler.CreateUriNode(It.IsAny<Uri>()))
                       .Returns((Uri u) => CreateMockedUriNode(u));

            _refObjMap.Setup(map => map.SubjectMap).Returns(_subjectMap.Object);
            _refObjMap.Setup(map => map.PredicateObjectMap).Returns(_predObjectMap.Object);

            _processor = new W3CRefObjectMapProcessor(_termGenerator.Object)
                {
                    Log = _log.Object
                };
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
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<INode>(_subjectMap.Object, It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void GeneratesObjectForEachLogicalRow(int rowsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<INode>(_subjectMap.Object, It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns)),
                Times.Exactly(rowsCount));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(9, 1)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(9, 5)]
        public void GeneratesTermForPredicatesForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(9, 1)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(9, 5)]
        public void GeneratesTermForSubjectsGraphMapsForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _subjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(9, 1)]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(9, 5)]
        public void GeneratesTermForPredicateObjectGraphMapsForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 3, 0, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(0, 0, 0, 2)]
        [TestCase(2, 7, 0, 0)]
        [TestCase(4, 7, 3, 0)]
        [TestCase(2, 9, 0, 2)]
        [TestCase(3, 8, 4, 2)]
        public void AssertsTriplesForEachCombinationOfPredicatesAndOptionallyGraphs(int rowsCount, int predicatesCount, int predObjectGraphsCount, int subjectGraphsCount)
        {
            // given
            int expectedCallsCount = predObjectGraphsCount == 0 && subjectGraphsCount == 0
                                         ? rowsCount * predicatesCount
                                         : rowsCount * predicatesCount * (subjectGraphsCount + predObjectGraphsCount);

            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.PredicateMaps).Returns(() => GenerateNMocks<IPredicateMap>(predicatesCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(predObjectGraphsCount));
            _subjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(subjectGraphsCount));
            _termGenerator.Setup(gen => gen.GenerateTerm<INode>(It.IsAny<ITermMap>(), It.IsAny<IDataRecord>()))
                          .Returns(() => CreateMockedUriNode(new Uri("http://www.exampl.com/node")));
            _termGenerator.Setup(gen => gen.GenerateTerm<IUriNode>(It.IsAny<ITermMap>(), It.IsAny<IDataRecord>()))
                          .Returns(() => CreateMockedUriNode(new Uri("http://www.exampl.com/node")));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.IsAny<Triple>()), Times.Exactly(expectedCallsCount));
        }

        [Test]
        public void LogsSqlExecuteError()
        {
            // given
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.Setup(com => com.ExecuteReader()).Throws(new Exception("Error message"));
            _connection.Setup(con => con.CreateCommand()).Returns(command.Object);

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _log.Verify(log => log.LogQueryExecutionError(_refObjMap.Object, "Error message"), Times.Once());
        }
    }
}