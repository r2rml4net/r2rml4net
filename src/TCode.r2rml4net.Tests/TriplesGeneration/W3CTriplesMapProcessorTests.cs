using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;
using System.Linq.Expressions;

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
            _triplesMapProcessor = new W3CTriplesMapProcessor(_termGenerator.Object)
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
            Assert.Throws<InvalidMapException>(() => _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object));

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
            subjectMap.Setup(sm => sm.GraphMaps).Returns(new IGraphMap[0]);
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

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
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

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
            subjectMap.Setup(sm => sm.GraphMaps).Returns(GenerateNMocks<IGraphMap>(graphsCount));
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(rowsCount));

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

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
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

            // then
            _predicateObjectMapProcessor.Verify(
                proc => proc.ProcessPredicateObjectMap(It.IsAny<INode>(), It.IsAny<IPredicateObjectMap>(), It.IsAny<IEnumerable<IUriNode>>(), It.IsAny<IDataRecord>(), _rdfHandler.Object),
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
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(10));
            _triplesMap.Setup(map => map.PredicateObjectMaps).Returns(new[] { predicateObjectMap.Object });
            predicateObjectMap.Setup(map => map.RefObjectMaps).Returns(() =>
                GenerateNMocks<IRefObjectMap>(refObjectMapsCount,
                new Tuple<Expression<Func<IRefObjectMap, object>>, Func<object>>(map => map.SubjectMap, () => subjectMap.Object))
                );

            // when
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

            // then
            _refObjectMapProcessor.Verify(proc =>
                proc.ProcessRefObjectMap(
                    It.IsAny<IRefObjectMap>(), subjectMap.Object,
                    _connection.Object, 5, _rdfHandler.Object),
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
            _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object);

            // then
            _refObjectMapProcessor.Verify(proc =>
                proc.ProcessRefObjectMap(
                    It.IsAny<IRefObjectMap>(), subjectMap.Object,
                    _connection.Object, 5, _rdfHandler.Object),
                Times.Never());
        }

        [Test]
        public void LogsSqlExecuteErrorAndThrows()
        {
            // given
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.Setup(com => com.ExecuteReader()).Throws(new Exception("Error message"));
            _connection.Setup(con => con.CreateCommand()).Returns(command.Object);
            _triplesMap.Setup(map => map.SubjectMap).Returns(new Mock<ISubjectMap>().Object);

            // when
            Assert.Throws<InvalidMapException>(() => _triplesMapProcessor.ProcessTriplesMap(_triplesMap.Object, _connection.Object, _rdfHandler.Object));

            // then
            _log.Verify(log => log.LogQueryExecutionError(_triplesMap.Object, "Error message"), Times.Once());
        }
    }
}