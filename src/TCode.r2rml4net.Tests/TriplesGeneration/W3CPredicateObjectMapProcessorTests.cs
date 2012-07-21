using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    public class W3CPredicateObjectMapProcessorTests
    {
        private W3CPredicateObjectMapProcessor _processor;
        private Mock<IPredicateObjectMap> _predicateObjectMap;
        private Mock<IDataRecord> _logicalRow;
        private Mock<IRDFTermGenerator> _termGenerator;
        private Mock<IRdfHandler> _storeWriter;

        [SetUp]
        public void Setup()
        {
            _predicateObjectMap = new Mock<IPredicateObjectMap>();
            _logicalRow= new Mock<IDataRecord>();
            _termGenerator = new Mock<IRDFTermGenerator>();
            _storeWriter = new Mock<IRdfHandler>();
            _processor = new W3CPredicateObjectMapProcessor(_termGenerator.Object, _storeWriter.Object);
        }
    }
}