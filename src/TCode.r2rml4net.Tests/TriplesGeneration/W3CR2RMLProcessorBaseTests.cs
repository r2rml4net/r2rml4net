using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CR2RMLProcessorBaseTests
    {
        private Mock<W3CR2RMLProcessorBase> _triplesGenerator;
        private Mock<IR2RML> _r2RML;
        private Mock<DbConnection> _connection;
        private Mock<ITriplesMapProcessor> _triplesMapProcessor;

        [SetUp]
        public void Setup()
        {
            _r2RML = new Mock<IR2RML>();
            _connection = new Mock<DbConnection>();
            _triplesMapProcessor = new Mock<ITriplesMapProcessor>();
            _triplesGenerator = new Mock<W3CR2RMLProcessorBase>(_connection.Object, _triplesMapProcessor.Object)
                                    {
                                        CallBase = true
                                    };
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void ProcessesAllTriplesMaps(int triplesMapsCount)
        {
            // given
            var triplesMaps = GenerateTriplesMaps(triplesMapsCount).ToList();
            _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
            _triplesMapProcessor.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>()));

            // when
            _triplesGenerator.Object.GenerateTriples(_r2RML.Object);

            // then
            _r2RML.Verify(rml => rml.TriplesMaps, Times.Once());
            _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>()), Times.Exactly(triplesMapsCount));
            foreach (var triplesMap in triplesMaps)
            {
                ITriplesMap map = triplesMap;
                _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(map, It.IsAny<DbConnection>()), Times.Once());
            }
        }

        IEnumerable<ITriplesMap> GenerateTriplesMaps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Mock<ITriplesMap>().Object;
            }
        } 
    }
}
