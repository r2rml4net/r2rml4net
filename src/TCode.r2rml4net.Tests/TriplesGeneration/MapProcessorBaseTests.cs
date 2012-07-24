using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
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

        [SetUp]
        public void Setup()
        {
            _subject = new Mock<INode>().Object;
            _predicates = new[] { new Mock<IUriNode>().Object };
            _objects = new[] { new Mock<INode>().Object };
            _graphs = new IUriNode[0];

            _termGenerator = new Mock<IRDFTermGenerator>();

            _rdfHandler = new Mock<IRdfHandler>();
            _rdfHandler.Setup(writer => writer.CreateUriNode(It.IsAny<Uri>())).Returns((Uri uri) => CreateMockedUriNode(uri));

            _processor = new Mock<MapProcessorBase>(_termGenerator.Object, _rdfHandler.Object)
                             {
                                 CallBase = true
                             };
        }

        [Test]
        public void AddsToDefaultGraphWhenNoGraphSpecified()
        {
            // when
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs);

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
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
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
            _processor.Object.AddTriplesToDataSet(_subject, _predicates, _objects, _graphs);

            // then
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
            _rdfHandler.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)), Times.Once());
        }
    }
}
