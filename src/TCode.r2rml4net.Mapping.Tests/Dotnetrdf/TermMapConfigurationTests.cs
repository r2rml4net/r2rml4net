using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class TermMapConfigurationTests
    {
        INode _triplesMapNode;
        TermMapConfiguration _termMapConfiguration;
        private IGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = new R2RMLConfiguration().R2RMLMappings;
            _triplesMapNode = _graph.CreateUriNode(new Uri("http://mapping.com/SomeMap"));
            _termMapConfiguration = new Mock<TermMapConfiguration>(_triplesMapNode, _graph)
                                        {
                                            CallBase = true
                                        }.Object;
        }

        [Test]
        public void ConstructorCreatesNodeForTheTermMapIsRepresentsAndItsRelationToTriplesMap()
        {
            // then
            Assert.IsNotNull(_termMapConfiguration.TermMapNode);
            Assert.AreSame(_graph, _termMapConfiguration.TermMapNode.Graph);

            var triples = _termMapConfiguration.R2RMLMappings.GetTriplesWithSubject(_triplesMapNode).ToArray();
            Assert.AreEqual(1, triples.Count());
            Assert.AreSame(_termMapConfiguration.TermMapNode, triples.First().Object);
            Assert.AreEqual(_termMapConfiguration.R2RMLMappings.CreateUriNode("rr:subjectMap"), triples.First().Predicate);
        }

        [Test]
        public void CannotSetTermTypeTwice()
        {
            // when
            _termMapConfiguration.TermType.IsBlankNode();

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.TermType);
        }
    }
}
