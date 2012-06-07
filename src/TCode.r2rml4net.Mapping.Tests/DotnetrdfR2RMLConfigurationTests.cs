using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests
{
    [TestFixture]
    public class DotnetrdfR2RMLConfigurationTests
    {
        private Mock<Graph> _graph;
        private R2RMLConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _graph = new Mock<Graph>
                         {
                             CallBase = true
                         };
            _configuration = new R2RMLConfiguration();// {R2RMLMappings = _graph.Object};
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(20)]
        public void CanCreateTriplesMapsFromConcreteTable(int numberOfTables)
        {
            IList<ITriplesMapConfiguration> tripleMaps = new List<ITriplesMapConfiguration>(numberOfTables);

            for (int i = 0; i < numberOfTables; i++)
            {
                tripleMaps.Add(_configuration.CreateTriplesMapFromTable("TableName"));
            }

            Assert.IsTrue(tripleMaps.All(map => map != null));
            foreach (var configuration in tripleMaps)
            {
                Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(20)]
        public void CanCreateTriplesMapsFromR2RMLView(int numberOfTables)
        {
            IList<ITriplesMapConfiguration> tripleMaps = new List<ITriplesMapConfiguration>(numberOfTables);

            for (int i = 0; i < numberOfTables; i++)
            {
                tripleMaps.Add(_configuration.CreateTriplesMapFromR2RMLView("SELECT * from Table"));
            }

            Assert.IsTrue(tripleMaps.All(map => map != null));
            foreach (var configuration in tripleMaps)
            {
                Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            }
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullTableName(string tableName)
        {
            _configuration.CreateTriplesMapFromTable(tableName);
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullSqlQuery(string sqlQuery)
        {
            _configuration.CreateTriplesMapFromR2RMLView(sqlQuery);
        }

        [Test]
        public void SqlVersionUriCanBeChanged()
        {
            ITriplesMapConfiguration configuration = _configuration.CreateTriplesMapFromR2RMLView("SELECT...")
                                                                   .SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"));

            Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }

        [Test]
        public void SqlVersionUriCanBeChangedFromUriString()
        {
            ITriplesMapConfiguration configuration = _configuration.CreateTriplesMapFromR2RMLView("SELECT...")
                                                                   .SetSqlVersion("rr:SQL2008");

            Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }

        [Test]
        public void CreatingTriplesMapFromTableNameAssertsTriplesInGraph()
        {
            // given
            const string tablename = "TableName";
            string triplesMapUri = string.Format("{0}{1}TriplesMap", _configuration.R2RMLMappings.BaseUri, tablename);

            // when
            _configuration.CreateTriplesMapFromTable(tablename);

            // then
            //_graph.Verify(g => g.CreateUriNode("rr:TriplesMap"), Times.Once());
            //_graph.Verify(g => g.CreateUriNode("rdf:Type"), Times.Once());
            //_graph.Verify(g => g.CreateUriNode("rr:logicalTable"), Times.Once());
            //_graph.Verify(g => g.CreateUriNode("rr:tableName"), Times.Once());
            //_graph.Verify(g => g.CreateLiteralNode(tablename), Times.Once());
            //_graph.Verify(g => g.CreateBlankNode(), Times.Once());

            AssertTripleAssertion(triplesMapUri, RdfType, RrTriplesMapClass);
            //AssertTripleAssertionWithBlankNodeObject(triplesMapUri, RrLogicalTableProperty);
            //AssertTripleAssertionWithBlankSubjectAndLiteralNode(RrTableNameProperty, tablename);
        }

        [Test]
        public void ConfigurationBuilderCreatedWithAnEmptyGraph()
        {
            Assert.IsTrue(_configuration.R2RMLMappings.IsEmpty);
        }

        [Test]
        public void ConfigurationBuilderCreatedWithGraphWithDefaultNamespaces()
        {
            Assert.IsTrue(_configuration.R2RMLMappings.NamespaceMap.HasNamespace("rr"));
            Assert.AreEqual("http://www.w3.org/ns/r2rml#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rr").ToString());

            Assert.IsTrue(_configuration.R2RMLMappings.NamespaceMap.HasNamespace("rdf"));
            Assert.AreEqual("http://www.w3.org/1999/02/22-rdf-syntax-ns#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rdf").ToString());
        }

        [Test]
        public void ConfigurationBuilderConstructedWithDefaultBaseUri()
        {
            Assert.AreEqual(R2RMLConfiguration.DefaultBaseUri, _configuration.R2RMLMappings.BaseUri);
        }

        [Test]
        public void ConfigurationBuilderCanBeConstructedWithChangedDefaultBaseUri()
        {
            Uri baseUri = new Uri("http://this.is.test.com/rdf/");

            _configuration = new R2RMLConfiguration(baseUri);

            Assert.AreEqual(baseUri, _configuration.R2RMLMappings.BaseUri);
        }

        #region Assertion helper methods

        private void AssertGraphHasNode(string uri)
        {
            Assert.IsNotNull(_configuration.R2RMLMappings.GetUriNode(new Uri(uri)), string.Format("Node <{0}> not found in graph {1}", uri, _configuration.R2RMLMappings));
        }

        private void AssertTripleAssertion(string subjectUri, string predicateUri, string objectUri)
        {
            AssertGraphHasNode(subjectUri);
            AssertGraphHasNode(predicateUri);
            AssertGraphHasNode(objectUri);

            Assert.IsTrue(_configuration.R2RMLMappings.ContainsTriple(new Triple(
                _configuration.R2RMLMappings.GetUriNode(new Uri(subjectUri)),
                _configuration.R2RMLMappings.GetUriNode(new Uri(predicateUri)),
                _configuration.R2RMLMappings.GetUriNode(new Uri(objectUri))
                )), string.Format("Triple <{0}> => <{1}> => <{2}> not found in graph", subjectUri, predicateUri, objectUri));
        }

        private void AssertTripleAssertionWithBlankNodeObject(string subjectUri, string predicateUri)
        {
            _graph.Verify(g => g.Assert(
                It.Is<UriNode>(node => node.Uri.ToString() == subjectUri),
                It.Is<UriNode>(node => node.Uri.ToString() == predicateUri),
                It.IsAny<BlankNode>()
                ));
        }

        private void AssertTripleAssertionWithBlankNodeSubject(string predicateUri, string objectUri)
        {
            _graph.Verify(g => g.Assert(
                It.IsAny<BlankNode>(),
                It.Is<UriNode>(node => node.Uri.ToString() == predicateUri),
                It.Is<UriNode>(node => node.Uri.ToString() == objectUri)
                ));
        }

        private void AssertTripleAssertionWithLiteralNode(string subjectUri, string predicateUri, string literalValue)
        {
            _graph.Verify(g => g.Assert(
                It.Is<UriNode>(node => node.Uri.ToString() == subjectUri),
                It.Is<UriNode>(node => node.Uri.ToString() == predicateUri),
                It.Is<LiteralNode>(node => node.Value == literalValue)
                ));
        }

        private void AssertTripleAssertionWithBlankSubjectAndLiteralNode(string predicateUri, string literalValue)
        {
            _graph.Verify(g => g.Assert(
                It.IsAny<BlankNode>(),
                It.Is<UriNode>(node => node.Uri.ToString() == predicateUri),
                It.Is<LiteralNode>(node => node.Value == literalValue)
                ));
        }

        #endregion

        #region Uri constants

        private const string RrTriplesMapClass = "http://www.w3.org/ns/r2rml#TriplesMap";
        private const string RrLogicalTableProperty = "http://www.w3.org/ns/r2rml#logicalTable";
        private const string RrTableNameProperty = "http://www.w3.org/ns/r2rml#tableName";

        private const string RdfType = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        #endregion
    }
}
