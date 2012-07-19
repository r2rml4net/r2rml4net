using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CTriplesGeneratorBaseTests
    {
        private Mock<W3CTriplesGeneratorBase> _triplesGenerator;
        private Mock<IR2RML> _r2RML;

        [SetUp]
        public void Setup()
        {
            _r2RML = new Mock<IR2RML>();
            _triplesGenerator = new Mock<W3CTriplesGeneratorBase>
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
            _triplesGenerator.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>()));

            // when
            _triplesGenerator.Object.GenerateTriples(_r2RML.Object);

            // then
            _r2RML.Verify(rml => rml.TriplesMaps, Times.Once());
            _triplesGenerator.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>()), Times.Exactly(triplesMapsCount));
            foreach (var triplesMap in triplesMaps)
            {
                ITriplesMap map = triplesMap;
                _triplesGenerator.Verify(rml => rml.ProcessTriplesMap(map), Times.Once());
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
