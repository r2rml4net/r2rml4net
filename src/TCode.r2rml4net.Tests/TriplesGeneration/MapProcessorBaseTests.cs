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

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class MapProcessorBaseTests : TriplesGenerationTestsBase
    {
        private Mock<IRdfHandler> _rdfHandler;
        private Mock<MapProcessorBase> _processor;
        private INode _subject;
        private IEnumerable<IUriNode> _predicates;
        private IEnumerable<IUriNode> _graphs;
        private IEnumerable<INode> _objects;
        private Mock<IRDFTermGenerator> _termGenerator;
        private Mock<LogFacadeBase> _log;

        public MapProcessorBaseTests()
        {
            _subject = new Mock<INode>().Object;
            _predicates = new[] { new Mock<IUriNode>().Object };
            _objects = new[] { new Mock<INode>().Object };
            _graphs = new IUriNode[0];

            _termGenerator = new Mock<IRDFTermGenerator>();
            _log = new Mock<LogFacadeBase>();

            _rdfHandler = new Mock<IRdfHandler>();
            _rdfHandler.Setup(writer => writer.CreateUriNode(It.IsAny<Uri>())).Returns((Uri uri) => CreateMockedUriNode(uri));

            _processor = new Mock<MapProcessorBase>(_termGenerator.Object)
                             {
                                 CallBase = true
                             };
            _processor.Object.Log = _log.Object;
        }

        [Fact]
        public void AddsToDefaultGraphWhenNoGraphSpecified()
        {
            // when
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
        }

        [Fact]
        public void AddsToDefaultGraphWhenSpecialGraphSpecified()
        {
            // given
            _graphs = new[]
                          {
                              CreateMockedUriNode(new Uri("http://www.w3.org/ns/r2rml#defaultGraph"))
                          };

            // when
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
        }

        [Fact]
        public void DoesNotAssertAnyTriplesIfAnyTermIsNull()
        {
            // given
            _rdfHandler = new Mock<IRdfHandler>(MockBehavior.Strict);
            _rdfHandler.Setup(writer => writer.CreateUriNode(new Uri("http://www.w3.org/ns/r2rml#defaultGraph"))).Returns((Uri uri) => CreateMockedUriNode(uri));

            // when
            _processor.Object.AddTriplesToDataSet(null, GenerateNMocks<IUriNode>(1), GenerateNMocks<INode>(1), new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, GenerateNMocks<IUriNode>(1), new INode[] { null }, new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, new IUriNode[] { null }, GenerateNMocks<INode>(1), new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, new IUriNode[] { null }, GenerateNMocks<INode>(1), new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, GenerateNMocks<IUriNode>(1), new INode[] { null }, new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, new IUriNode[] { null }, new INode[] { null }, new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, new IUriNode[] { null }, new INode[] { null }, new IUriNode[0], _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, GenerateNMocks<IUriNode>(1), GenerateNMocks<INode>(1), new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, GenerateNMocks<IUriNode>(1), new INode[] { null }, new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, new IUriNode[] { null }, GenerateNMocks<INode>(1), new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, new IUriNode[] { null }, GenerateNMocks<INode>(1), new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, GenerateNMocks<IUriNode>(1), new INode[] { null }, new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(_subject, new IUriNode[] { null }, new INode[] { null }, new IUriNode[] { null }, _rdfHandler.Object);
            _processor.Object.AddTriplesToDataSet(null, new IUriNode[] { null }, new INode[] { null }, new IUriNode[] { null }, _rdfHandler.Object);

            // then
            _rdfHandler.VerifyAll();
        }

        [Fact]
        public void CanHaveSpecialDefaultGraphMixedWithRealGraphs()
        {
            // given
            _graphs = new[]
                          {
                              CreateMockedUriNode(new Uri("http://www.w3.org/ns/r2rml#defaultGraph")),
                              CreateMockedUriNode(new Uri("http://www.example.com/someGraph"))
                          };

            // when
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
        }

        [Fact]
        public void ThrowsErrorOnInvalidSqlAlndLogsIt()
        {
            // given
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var map = new Mock<IQueryMap>();
            connection.Setup(con => con.CreateCommand()).Returns(command.Object);
            command.Setup(cmd => cmd.ExecuteReader()).Throws<Exception>();

            // when
            IDataReader reader;
            Assert.Throws<InvalidMapException>(() => _processor.Object.FetchLogicalRows(connection.Object, map.Object, out reader));

            // then
            _log.Verify(log => log.LogQueryExecutionError(It.IsAny<IQueryMap>(), It.IsAny<string>()));
        }

        [Fact]
        public void ThrowsOnMultipleColumnsWithSameName()
        {
            // given
            Mock<IDataReader> reader = new Mock<IDataReader>(MockBehavior.Strict);
            reader.Setup(rdr => rdr.FieldCount).Returns(3);
            reader.Setup(rdr => rdr.GetName(0)).Returns("Id");
            reader.Setup(rdr => rdr.GetName(1)).Returns("Name");
            reader.Setup(rdr => rdr.GetName(2)).Returns("Id");

            // when
            Assert.Throws<InvalidMapException>(() => _processor.Object.AssertNoDuplicateColumnNames(reader.Object));

            // then
            reader.Verify(rdr => rdr.GetName(It.IsAny<int>()), Times.Exactly(3));
            reader.Verify(rdr => rdr.FieldCount, Times.Once());
        }
    }
}
