#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;
using System.Linq.Expressions;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class W3CTriplesMapProcessorTests : TriplesGenerationTestsBase
    {
        private readonly W3CTriplesMapProcessor _triplesMapProcessor;
        private readonly Mock<ITriplesMap> _triplesMap;
        private readonly Mock<IDbConnection> _connection;
        private readonly Mock<LogFacadeBase> _log;
        private readonly Mock<IRDFTermGenerator> _termGenerator;
        private readonly Mock<IRdfHandler> _rdfHandler;
        private readonly Mock<IPredicateObjectMapProcessor> _predicateObjectMapProcessor;
        private readonly Mock<IRefObjectMapProcessor> _refObjectMapProcessor;

        public W3CTriplesMapProcessorTests()
        {
            _log = new Mock<LogFacadeBase>();
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

        [Fact]
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
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

        [Theory]
        [InlineData(0, 5)]
        [InlineData(1, 0)]
        [InlineData(1, 5)]
        [InlineData(10, 5)]
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

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 5)]
        [InlineData(10, 5)]
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

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(17)]
        public void ProcessesEachRefObjectMapFromPredicateObjectMap(int refObjectMapsCount)
        {
            // given
            Mock<ISubjectMap> subjectMap = new Mock<ISubjectMap>();
            Mock<IPredicateObjectMap> predicateObjectMap = new Mock<IPredicateObjectMap>();
            _triplesMap.Setup(proc => proc.SubjectMap).Returns(subjectMap.Object);
            _connection.Setup(conn => conn.CreateCommand()).Returns(CreateCommandWithNRowsResult(10));
            _triplesMap.Setup(map => map.PredicateObjectMaps).Returns(new[] { predicateObjectMap.Object });
            predicateObjectMap.Setup(map => map.RefObjectMaps).Returns(() =>
                GenerateNMocks<IRefObjectMap, ISubjectMap>(refObjectMapsCount,
                new Tuple<Expression<Func<IRefObjectMap, ISubjectMap>>, Func<ISubjectMap>>(map => map.SubjectMap, () => subjectMap.Object))
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void SettingLogShouldSetLogToRdfTermGenerator()
        {
            // given
            Mock<LogFacadeBase> log = new Mock<LogFacadeBase>();

            // when
            _triplesMapProcessor.Log = log.Object;

            // then
            _termGenerator.VerifySet(g => g.Log = log.Object, Times.Once());
        }
    }
}