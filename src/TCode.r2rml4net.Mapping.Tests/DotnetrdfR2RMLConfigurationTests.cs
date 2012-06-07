using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;

namespace TCode.r2rml4net.Mapping.Tests
{
    [TestFixture]
    public class DotnetrdfR2RMLConfigurationTests
    {
        private DotnetrdfR2RMLConfiguration _configuration;

        public void Setup()
        {
            _configuration = new DotnetrdfR2RMLConfiguration();
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
                Assert.IsInstanceOf<ITriplesMapConfiguration>(configuration);
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

            Assert.IsInstanceOf<ITriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }

        [Test]
        public void SqlVersionUriCanBeChangedFromUriString()
        {
            ITriplesMapConfiguration configuration = _configuration.CreateTriplesMapFromR2RMLView("SELECT...")
                                                                   .SetSqlVersion("rr:SQL2008");

            Assert.IsInstanceOf<ITriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }
    }
}
