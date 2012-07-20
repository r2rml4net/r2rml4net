using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class TriplesMapProcessorTests
    {
        private TriplesMapProcessor _triplesMapProcessor;
        private Mock<ITriplesMap> _triplesMap;
        private Mock<IDbConnection> _connection;
        private Mock<ITriplesGenerationLog> _log;
        private Mock<IRDFTermGenerator> _termGenerator;

        [SetUp]
        public void Setup()
        {
            _log = new Mock<ITriplesGenerationLog>();
            _triplesMap = new Mock<ITriplesMap>();
            _connection = new Mock<IDbConnection>();
            _termGenerator = new Mock<IRDFTermGenerator>();
            _triplesMapProcessor = new TriplesMapProcessor(_termGenerator.Object)
                                       {
                                           Log = _log.Object
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
            _termGenerator.Verify(tg => tg.GenerateTerm(subjectMap.Object, It.IsAny<IDataRecord>()), Times.Exactly(rowsCount));
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
        [TestCase(1, 5)]
        [TestCase(10, 5)]
        public void CreatesTermForEachGraphForEachLogicalRow(int rowsCount, int graphsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            subjectMap.Setup(sm => sm.Graphs).Returns(MockGraphMaps(graphsCount));
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object);

            // then
            _termGenerator.Verify(tg => tg.GenerateTerm(It.IsAny<IGraphMap>(), It.IsAny<IDataRecord>()), Times.Exactly(rowsCount * graphsCount));
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

        private static IEnumerable<IGraphMap> MockGraphMaps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Mock<IGraphMap>().Object;
            }
        }
    }
}