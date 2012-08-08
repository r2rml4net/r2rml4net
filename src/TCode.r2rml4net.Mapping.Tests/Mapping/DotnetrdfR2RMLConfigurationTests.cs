using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class DotnetrdfR2RMLConfigurationTests
    {
        private R2RMLConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
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
            Assert.AreEqual(triplesMapUri, triplesMap.Uri.AbsoluteUri);
            _configuration.R2RMLMappings.VerifyHasTriple(triplesMapUri, UriConstants.RdfType, UriConstants.RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankObject(triplesMapUri, UriConstants.RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(UriConstants.RrTableNameProperty, tablename);
        }

        [Test]
        public void CreatingTriplesMapFromR2RMLViewAssertsTriplesInGraph()
        {
            // given
            const string sqlQuery = "SELECT * from X";

            // when
            _configuration.CreateTriplesMapFromR2RMLView(sqlQuery);

            // then
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RdfType, UriConstants.RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndObject(UriConstants.RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(UriConstants.RrSqlQueryProperty, sqlQuery);
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
            Assert.AreEqual("http://www.w3.org/ns/r2rml#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rr").AbsoluteUri);

            Assert.IsTrue(_configuration.R2RMLMappings.NamespaceMap.HasNamespace("rdf"));
            Assert.AreEqual("http://www.w3.org/1999/02/22-rdf-syntax-ns#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rdf").AbsoluteUri);
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
    }
}
