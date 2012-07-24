using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class RDFTermGeneratorTests
    {
        private RDFTermGenerator _termGenerator;
        private Mock<ITermMap> _termMap;

        [SetUp]
        public void Setup()
        {
            _termMap = new Mock<ITermMap>();
            _termGenerator = new RDFTermGenerator();
        }
    }
}