using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CPredicateObjectMapProcessorTests : TriplesGenerationTestsBase
    {
        private W3CPredicateObjectMapProcessor _processor;
        private Mock<IPredicateObjectMap> _predicateObjectMap;
        private Mock<IDataRecord> _logicalRow;
        private Mock<IRDFTermGenerator> _termGenerator;
        private Mock<IRdfHandler> _storeWriter;
        private IEnumerable<IUriNode> _subjectGraphs;
        private IUriNode _subject;

        [SetUp]
        public void Setup()
        {
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
            _logicalRow = new Mock<IDataRecord>();
            _termGenerator = new Mock<IRDFTermGenerator>();
            _storeWriter = new Mock<IRdfHandler>();
            _subjectGraphs = new IUriNode[0];
            _subject = new Mock<IUriNode>().Object;
            _storeWriter.Setup(writer => writer.CreateUriNode(It.IsAny<Uri>())).Returns((Uri uri) => CreateMockdUriNode(uri));
            _processor = new W3CPredicateObjectMapProcessor(_termGenerator.Object, _storeWriter.Object);
        }

        private IUriNode CreateMockdUriNode(Uri uri)
        {
            var uriNode = new Mock<IUriNode>();
            uriNode.Setup(n => n.Uri).Returns(uri);
            return uriNode.Object;
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void GeneratesRDFTermsForEachPredicateMap(int mapsCount)
        {
            // given
            var predicateMaps = GenerateNMocks<IPredicateMap>(mapsCount).ToList();
            _predicateObjectMap.Setup(map => map.PredicateMaps).Returns(predicateMaps);

            // when
            _processor.ProcessPredicateObjectMap(_subject, _predicateObjectMap.Object, _subjectGraphs, _logicalRow.Object);

            // then
            _termGenerator.Verify(gen => gen.GenerateTerm<IUriNode>(It.IsAny<IPredicateMap>(), _logicalRow.Object), Times.Exactly(mapsCount));
            foreach (IPredicateMap predicateMap in predicateMaps)
            {
                IPredicateMap map = predicateMap;
                _termGenerator.Verify(gen => gen.GenerateTerm<IUriNode>(map, _logicalRow.Object));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void GeneratesRDFTermsForEachObjectMap(int mapsCount)
        {
            // given
            var objectMaps = GenerateNMocks<IObjectMap>(mapsCount).ToList();
            _predicateObjectMap.Setup(map => map.ObjectMaps).Returns(objectMaps);

            // when
            _processor.ProcessPredicateObjectMap(_subject, _predicateObjectMap.Object, _subjectGraphs, _logicalRow.Object);

            // then
            _termGenerator.Verify(gen => gen.GenerateTerm<INode>(It.IsAny<IObjectMap>(), _logicalRow.Object), Times.Exactly(mapsCount));
            foreach (IObjectMap objectMap in objectMaps)
            {
                IObjectMap map = objectMap;
                _termGenerator.Verify(gen => gen.GenerateTerm<INode>(map, _logicalRow.Object));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void GeneratesRDFTermsForEachGraphMap(int mapsCount)
        {
            // given
            var graphMaps = GenerateNMocks<IGraphMap>(mapsCount).ToList();
            _predicateObjectMap.Setup(map => map.GraphMaps).Returns(graphMaps);

            // when
            _processor.ProcessPredicateObjectMap(_subject, _predicateObjectMap.Object, _subjectGraphs, _logicalRow.Object);

            // then
            _termGenerator.Verify(gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), _logicalRow.Object), Times.Exactly(mapsCount));
            foreach (IGraphMap graphMap in graphMaps)
            {
                IGraphMap map = graphMap;
                _termGenerator.Verify(gen => gen.GenerateTerm<IUriNode>(map, _logicalRow.Object));
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(1, 8)]
        [TestCase(3, 7)]
        public void AssertsTriplesAccordingToObjectsAndPredicatesCount(int predicatesCount, int objectsCount)
        {
            // given
            var objectMaps = GenerateNMocks<IObjectMap>(objectsCount).ToList();
            _predicateObjectMap.Setup(map => map.ObjectMaps).Returns(objectMaps);
            var predicateMaps = GenerateNMocks<IPredicateMap>(predicatesCount).ToList();
            _predicateObjectMap.Setup(map => map.PredicateMaps).Returns(predicateMaps);
            _termGenerator.Setup(gen => gen.GenerateTerm<IUriNode>(It.IsAny<IPredicateMap>(), _logicalRow.Object))
                          .Returns(() => new Mock<IUriNode>().Object);
            _termGenerator.Setup(gen => gen.GenerateTerm<INode>(It.IsAny<IObjectMap>(), _logicalRow.Object))
                          .Returns(() => new Mock<INode>().Object);

            // when
            _processor.ProcessPredicateObjectMap(_subject, _predicateObjectMap.Object, _subjectGraphs, _logicalRow.Object);

            // then
            _storeWriter.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri == null)),
                                Times.Exactly(predicatesCount * objectsCount));
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(1, 8)]
        [TestCase(3, 7)]
        public void AssertsTriplesAccordingToGraphsCount(int subjectGrapsCount, int graphsCount)
        {
            // given
            var objectMaps = GenerateNMocks<IObjectMap>(3).ToList();
            _predicateObjectMap.Setup(map => map.ObjectMaps).Returns(objectMaps);
            var predicateMaps = GenerateNMocks<IPredicateMap>(7).ToList();
            _predicateObjectMap.Setup(map => map.PredicateMaps).Returns(predicateMaps);
            var graphMaps = GenerateNMocks<IGraphMap>(graphsCount).ToList();
            _predicateObjectMap.Setup(map => map.GraphMaps).Returns(graphMaps);
            _termGenerator.Setup(gen => gen.GenerateTerm<IUriNode>(It.IsAny<IPredicateMap>(), _logicalRow.Object))
                          .Returns(() => new Mock<IUriNode>().Object);
            _termGenerator.Setup(gen => gen.GenerateTerm<INode>(It.IsAny<IObjectMap>(), _logicalRow.Object))
                          .Returns(() => new Mock<INode>().Object);
            _termGenerator.Setup(gen => gen.GenerateTerm<IUriNode>(It.IsAny<IGraphMap>(), _logicalRow.Object))
                          .Returns(() =>
                          {
                              var mock = new Mock<IUriNode>();
                              mock.Setup(graph => graph.Uri).Returns(new Uri("http://www.example.com/graph"));
                              return mock.Object;
                          });
            _subjectGraphs = GenerateNMocks(subjectGrapsCount,
                new Tuple<Expression<Func<IUriNode, object>>, Func<object>>(map => map.Uri, () => new Uri("http://www.example.com/graph")));

            // when
            _processor.ProcessPredicateObjectMap(_subject, _predicateObjectMap.Object, _subjectGraphs, _logicalRow.Object);

            // then
            _storeWriter.Verify(handler => handler.HandleTriple(It.Is<Triple>(t => t.GraphUri != null)),
                                Times.Exactly(21 * (subjectGrapsCount + graphsCount)));
        }
    }
}