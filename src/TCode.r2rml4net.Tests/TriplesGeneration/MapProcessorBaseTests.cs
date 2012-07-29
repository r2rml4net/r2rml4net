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
    public class MapProcessorBaseTests : TriplesGenerationTestsBase
    {
        private Mock<IRdfHandler> _rdfHandler;
        private Mock<MapProcessorBase> _processor;
        private INode _subject;
        private IEnumerable<IUriNode> _predicates;
        private IEnumerable<IUriNode> _graphs;
        private IEnumerable<INode> _objects;
        private Mock<IRDFTermGenerator> _termGenerator;
        private Mock<ITriplesGenerationLog> _log;

        [SetUp]
        public void Setup()
        {
            _subject = new Mock<INode>().Object;
            _predicates = new[] { new Mock<IUriNode>().Object };
            _objects = new[] { new Mock<INode>().Object };
            _graphs = new IUriNode[0];

            _termGenerator = new Mock<IRDFTermGenerator>();
            _log = new Mock<ITriplesGenerationLog>();

            _rdfHandler = new Mock<IRdfHandler>();
            _rdfHandler.Setup(writer => writer.CreateUriNode(It.IsAny<Uri>())).Returns((Uri uri) => CreateMockedUriNode(uri));

            _processor = new Mock<MapProcessorBase>(_termGenerator.Object)
                             {
                                 CallBase = true
                             };
            _processor.Object.Log =_log.Object;
        }

        [Test]
        public void AddsToDefaultGraphWhenNoGraphSpecified()
        {
            // when
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs, _rdfHandler.Object);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void HandlesSqlExecuteErrorGracefully()
        {
            // given
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var map = new Mock<IQueryMap>();
            connection.Setup(con => con.CreateCommand()).Returns(command.Object);
            command.Setup(cmd => cmd.ExecuteReader()).Throws<Exception>();
            
            // when
            IDataReader reader;
            bool result = _processor.Object.FetchLogicalRows(connection.Object, map.Object, out reader);

            // then
            Assert.IsFalse(result);
            Assert.IsNull(reader);
        }
    }
}
