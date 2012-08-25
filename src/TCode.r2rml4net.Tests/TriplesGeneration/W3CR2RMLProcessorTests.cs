using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDF;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CR2RMLProcessorTests
    {
        private W3CR2RMLProcessor _triplesGenerator;
        private Mock<IR2RML> _r2RML;
        private Mock<DbConnection> _connection;
        private Mock<ITriplesMapProcessor> _triplesMapProcessor;
        private Mock<IRdfHandler> _rdfHandler;
        private bool? _handlingResult;

        [SetUp]
        public void Setup()
        {
            _rdfHandler = new Mock<IRdfHandler>();
            _rdfHandler.Setup(handler => handler.EndRdf(It.IsAny<bool>()))
                       .Callback((bool result) => _handlingResult = result);
            _r2RML = new Mock<IR2RML>();
            _connection = new Mock<DbConnection>();
            _triplesMapProcessor = new Mock<ITriplesMapProcessor>();
            _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object, new MappingOptions());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void ProcessesAllTriplesMaps(int triplesMapsCount)
        {
            // given
            var triplesMaps = GenerateTriplesMaps(triplesMapsCount).ToList();
            _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
            _triplesMapProcessor.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()));

            // when
            _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

            // then
            _r2RML.Verify(rml => rml.TriplesMaps, Times.Once());
            _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Exactly(triplesMapsCount));
            foreach (var triplesMap in triplesMaps)
            {
                ITriplesMap map = triplesMap;
                _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(map, It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
            }
            Assert.IsTrue(_handlingResult.HasValue && _handlingResult.Value);
            Assert.IsTrue(_triplesGenerator.Success);
        }

        IEnumerable<ITriplesMap> GenerateTriplesMaps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Mock<ITriplesMap>().Object;
            }
        }

        [Test]
        public void CanStopProcessingIfDataErrorOccurs()
        {
            // given
            var triplesMaps = GenerateTriplesMaps(3).ToList();
            _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
            _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object, new MappingOptions
            {
                IgnoreDataErrors = false
            });
            _triplesMapProcessor.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()))
                                .Throws(new InvalidTermException(new Mock<ITermMap>().Object, "error"));

            // when
            _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

            // then
            _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
            Assert.IsTrue(_handlingResult.HasValue && !_handlingResult.Value);
            Assert.IsFalse(_triplesGenerator.Success);
        }

        [Test]
        public void CanStopProcessingIfMappingErrorOccurs()
        {
            // given
            var triplesMaps = GenerateTriplesMaps(3).ToList();
            _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
            _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object, new MappingOptions
            {
                IgnoreMappingErrors = false
            });
            _triplesMapProcessor.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()))
                                .Throws(new InvalidMapException("error"));

            // when
            _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

            // then
            _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
            Assert.IsTrue(_handlingResult.HasValue && !_handlingResult.Value);
            Assert.IsFalse(_triplesGenerator.Success);
        }
    }
}
