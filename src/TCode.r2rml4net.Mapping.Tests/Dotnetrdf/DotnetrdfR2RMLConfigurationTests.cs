using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Dotnetrdf
{
    [TestFixture]
    public class DotnetrdfR2RMLConfigurationTests
    {
        private R2RMLConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            new Mock<Graph>
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
            var triplesMap = _configuration.CreateTriplesMapFromTable(tablename);

            // then
            Assert.AreEqual(triplesMapUri, triplesMap.Uri.ToString());
            _configuration.R2RMLMappings.VerifyHasTriple(triplesMapUri, RdfType, RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankObject(triplesMapUri, RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(RrTableNameProperty, tablename);
        }

        [Test]
        public void CreatingTriplesMapFromR2RMLViewAssertsTriplesInGraph()
        {
            // given
            const string sqlQuery = "SELECT * from X";

            // when
            var triplesMap = _configuration.CreateTriplesMapFromR2RMLView(sqlQuery);

            // then
            _configuration.R2RMLMappings.VerifyHasTriple(triplesMap.Uri, RdfType, RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankObject(triplesMap.Uri, RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(RrSqlQueryProperty, sqlQuery);
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

        [Test]
        public void CanCreateSubjectMaps()
        {
            // given
            ISubjectMapConfiguration subjectMapConfiguration = _configuration.CreateTriplesMapFromTable("Table").SubjectMap();

            // then
            Assert.IsNotNull(subjectMapConfiguration);
            Assert.IsInstanceOf<TermMapConfiguration>(subjectMapConfiguration);
        }

        #region Uri constants

        private const string RrTriplesMapClass = "http://www.w3.org/ns/r2rml#TriplesMap";
        private const string RrLogicalTableProperty = "http://www.w3.org/ns/r2rml#logicalTable";
        private const string RrTableNameProperty = "http://www.w3.org/ns/r2rml#tableName";
        private const string RrSqlQueryProperty = "http://www.w3.org/ns/r2rml#sqlQuery";

        private const string RdfType = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        #endregion
    }
}
