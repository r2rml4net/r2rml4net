using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    [TestFixture]
    class W3CRefObjectMapProcessorTests
    {
        private W3CRefObjectMapProcessor _processor;
        private Mock<IRdfHandler> _rdfHandler;
        private Mock<ISubjectMap> _subjectMap;
        private Mock<IRefObjectMap> _refObjMap;
        private Mock<IDbConnection> _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new Mock<IDbConnection>();
            _refObjMap = new Mock<IRefObjectMap>();
            _rdfHandler = new Mock<IRdfHandler>();
            _subjectMap = new Mock<ISubjectMap>();
            _processor = new W3CRefObjectMapProcessor(_rdfHandler.Object);
        }
    }
}