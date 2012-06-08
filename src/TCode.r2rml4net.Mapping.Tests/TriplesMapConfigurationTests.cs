using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        TriplesMapConfiguration _triplesMapConfiguration;

        [SetUp]
        public void Setup()
        {
            _triplesMapConfiguration = new TriplesMapConfiguration(new Graph());
        }

        [Test]
        public void SqlQueryIsSetAsIs()
        {
            // given
            const string query = "SELECT * from Table";

            // when
            _triplesMapConfiguration.SqlQuery = query;

            // then
            Assert.AreEqual(query, _triplesMapConfiguration.SqlQuery);
        }
    }
}
