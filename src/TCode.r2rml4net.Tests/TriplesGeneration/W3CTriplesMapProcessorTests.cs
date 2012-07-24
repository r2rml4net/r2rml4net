using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CTriplesMapProcessorTests : TriplesGenerationTestsBase
    {
        private W3CTriplesMapProcessor _triplesMapProcessor;
        private Mock<ITriplesMap> _triplesMap;
        private Mock<IDbConnection> _connection;
        private Mock<ITriplesGenerationLog> _log;
        private Mock<IRDFTermGenerator> _termGenerator;
        private Mock<IRdfHandler> _rdfHandler;
        private Mock<IPredicateObjectMapProcessor> _predicateObjectMapProcessor;
        private Mock<IRefObjectMapProcessor> _refObjectMapProcessor;

        [SetUp]
        public void Setup()
        {
            _log = new Mock<ITriplesGenerationLog>();
            _triplesMap = new Mock<ITriplesMap>();
            _connection = new Mock<IDbConnection>();
            _termGenerator = new Mock<IRDFTermGenerator>();
            _rdfHandler = new Mock<IRdfHandler>();
            _predicateObjectMapProcessor = new Mock<IPredicateObjectMapProcessor>();
            _refObjectMapProcessor = new Mock<IRefObjectMapProcessor>();
            _triplesMapProcessor = new W3CTriplesMapProcessor(_termGenerator.Object, _rdfHandler.Object)
                                       {
                                           Log = _log.Object,
                                           PredicateObjectMapProcessor = _predicateObjectMapProcessor.Object,
                                           RefObjectMapProcessor = _refObjectMapProcessor.Object
                                       };
        }

        [Test]
        public void DoesNotExecuteQueryIfSubjectMapIsInvalid()
        {
            // given
            _triplesMap.Setup(proc => proc.SubjectMap).Returns((ISubjectMap)null);

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _connection.Verify(conn => conn.CreateCommand(), Times.Never());
            _log.Verify(log => log.LogMissingSubject(It.IsAny<ITriplesMap>()), Times.Once());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void GeneratesTermForSubjectForEachForLogicalRow(int rowsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            subjectMap.Setup(sm => sm.Graphs).Returns(new IGraphMap[0]);
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _termGenerator.Verify(tg => tg.GenerateTerm<INode>(subjectMap.Object, It.IsAny<IDataRecord>()), Times.Exactly(rowsCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void ReadsSubjectClassesOnce(int rowsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            subjectMap.Setup(sm => sm.Classes).Returns(new Uri[0]);
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            subjectMap.Verify(sm => sm.Classes, Times.Once());
        }

        [TestCase(0, 5)]
        [TestCase(1, 0)]
        [TestCase(1, 5)]
        [TestCase(10, 5)]
        public void CreatesTermForEachGraphForEachLogicalRow(int rowsCount, int graphsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            subjectMap.Setup(sm => sm.Graphs).Returns(GenerateNMocks<IGraphMap>(graphsCount));
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _termGenerator.Verify(tg => tg.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.IsAny<IDataRecord>()), Times.Exactly(rowsCount * graphsCount));
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(1, 5)]
        [TestCase(10, 5)]
        public void ProcessesEachPredicateObjectMap(int mapsCount, int rowsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _triplesMap.Setup(tm => tm.PredicateObjectMaps).Returns(GenerateNMocks<IPredicateObjectMap>(mapsCount));
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _predicateObjectMapProcessor.Verify(
                proc => proc.ProcessPredicateObjectMap(It.IsAny<INode>(), It.IsAny<IPredicateObjectMap>(), It.IsAny<IEnumerable<IUriNode>>(), It.IsAny<IDataRecord>()), 
                Times.Exactly(mapsCount * rowsCount));
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(17)]
        public void ProcessesEachRefObjectMapFromPredicateObjectMap(int refObjectMapsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(1));
            _triplesMap.Setup(map => map.PredicateObjectMaps).Returns(new[] {predicateObjectMap.Object});
            predicateObjectMap.Setup(map => map.RefObjectMaps).Returns(() => GenerateNMocks<IRefObjectMap>(refObjectMapsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _refObjectMapProcessor.Verify(proc => 
                proc.ProcessRefObjectMap(
                    It.IsAny<IRefObjectMap>(),
                    _connection.Object,
                    It.IsAny<IEnumerable<IGraphMap>>()), 
                Times.Exactly(refObjectMapsCount));
        }

        [Test]
        public void SkipProcessingRefObjectMapIfSubjectMapIsAbsent()
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            Mock<IRefObjectMap> refObjectMap = new Mock<IRefObjectMap>();
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(1));
            _triplesMap.Setup(map => map.PredicateObjectMaps).Returns(new[] { predicateObjectMap.Object });
            predicateObjectMap.Setup(map => map.RefObjectMaps).Returns(() => new[] { refObjectMap.Object });

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _refObjectMapProcessor.Verify(proc => 
                proc.ProcessRefObjectMap(
                    It.IsAny<IRefObjectMap>(),
                    _connection.Object,
                    It.IsAny<IEnumerable<IGraphMap>>()), 
                Times.Never());
        }

        private static IDbCommand CreateCommandWithNRowsResult(int rowsCount)
        {
            int rowsReturned = 0;
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataReader> reader = new Mock<IDataReader>();
            command.Setup(cmd => cmd.ExecuteReader()).Returns(reader.Object);

            reader.Setup(rdr => rdr.Read()).Returns(() => rowsReturned++ < rowsCount);

            return command.Object;
        }
    }
}