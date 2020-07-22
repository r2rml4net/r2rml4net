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
using System.Data;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class W3CRefObjectMapProcessorTests : TriplesGenerationTestsBase
    {
        private readonly W3CRefObjectMapProcessor _processor;
        private readonly Mock<IRdfHandler> _rdfHandler;
        private readonly Mock<ISubjectMap> _subjectMap;
        private readonly Mock<IRefObjectMap> _refObjMap;
        private readonly Mock<IDbConnection> _connection;
        private readonly Mock<IRDFTermGenerator> _termGenerator;
        private readonly Mock<IPredicateObjectMap> _predObjectMap;
        private readonly Mock<ISubjectMap> _childSubjectMap;

        public W3CRefObjectMapProcessorTests()
        {
            _termGenerator = new Mock<IRDFTermGenerator>();
            _connection = new Mock<IDbConnection>();
            _refObjMap = new Mock<IRefObjectMap>();
            _rdfHandler = new Mock<IRdfHandler>();
            _subjectMap = new Mock<ISubjectMap>();
            _predObjectMap = new Mock<IPredicateObjectMap>();
            _childSubjectMap = new Mock<ISubjectMap>();

            _rdfHandler.Setup(handler => handler.CreateUriNode(It.IsAny<Uri>()))
                       .Returns((Uri u) => CreateMockedUriNode(u));

            _refObjMap.Setup(map => map.SubjectMap).Returns(_subjectMap.Object);
            _refObjMap.Setup(map => map.PredicateObjectMap).Returns(_predObjectMap.Object);

            _processor = new W3CRefObjectMapProcessor(_termGenerator.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(15)]
        public void GeneratesSubjectForEachLogicalRow(int rowsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<INode>(_childSubjectMap.Object, It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(15)]
        public void GeneratesObjectForEachLogicalRow(int rowsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<INode>(_subjectMap.Object, It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns)),
                Times.Exactly(rowsCount));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(9, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(9, 1)]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(9, 5)]
        public void GeneratesTermForPredicatesForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(9, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(9, 1)]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(9, 5)]
        public void GeneratesTermForSubjectsGraphMapsForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _childSubjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(9, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(9, 1)]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(9, 5)]
        public void GeneratesTermForPredicateObjectGraphMapsForEachRow(int rowsCount, int graphsCount)
        {
            // given
            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(graphsCount));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _termGenerator.Verify(
                gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), It.Is<ColumnConstrainedDataRecord>(rec => rec.LimitType == ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns)),
                Times.Exactly(rowsCount * graphsCount));
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 3, 0, 0)]
        [InlineData(0, 0, 1, 0)]
        [InlineData(0, 0, 0, 2)]
        [InlineData(2, 7, 0, 0)]
        [InlineData(4, 7, 3, 0)]
        [InlineData(2, 9, 0, 2)]
        [InlineData(3, 8, 4, 2)]
        public void AssertsTriplesForEachCombinationOfPredicatesAndOptionallyGraphs(int rowsCount, int predicatesCount, int predObjectGraphsCount, int subjectGraphsCount)
        {
            // given
            int expectedCallsCount = predObjectGraphsCount == 0 && subjectGraphsCount == 0
                                         ? rowsCount * predicatesCount
                                         : rowsCount * predicatesCount * (subjectGraphsCount + predObjectGraphsCount);

            _connection.Setup(c => c.CreateCommand()).Returns(() => CreateCommandWithNRowsResult(rowsCount));
            _predObjectMap.Setup(s => s.PredicateMaps).Returns(() => GenerateNMocks<IPredicateMap>(predicatesCount));
            _predObjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(predObjectGraphsCount));
            _childSubjectMap.Setup(s => s.GraphMaps).Returns(() => GenerateNMocks<IGraphMap>(subjectGraphsCount));
            _termGenerator.Setup(gen => gen.GenerateTerm<INode>(It.IsAny<ITermMap>(), It.IsAny<IDataRecord>()))
                          .Returns(() => CreateMockedUriNode(new Uri("http://www.exampl.com/node")));
            _termGenerator.Setup(gen => gen.GenerateTerm<IUriNode>(It.IsAny<ITermMap>(), It.IsAny<IDataRecord>()))
                          .Returns(() => CreateMockedUriNode(new Uri("http://www.exampl.com/node")));

            // when
            _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.IsAny<Triple>()), Times.Exactly(expectedCallsCount));
        }

        [Fact]
        public void LogsSqlExecuteErrorAndThrows()
        {
            // given
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.Setup(com => com.ExecuteReader()).Throws(new Exception("Error message"));
            _connection.Setup(con => con.CreateCommand()).Returns(command.Object);

            // then
            Assert.Throws<InvalidMapException>(() => _processor.ProcessRefObjectMap(_refObjMap.Object, _childSubjectMap.Object, _connection.Object, 2, _rdfHandler.Object));
        }
    }
}