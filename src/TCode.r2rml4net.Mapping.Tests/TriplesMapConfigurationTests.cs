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
            // Initialize TriplesMapConfiguration with a default graph
            _triplesMapConfiguration = new TriplesMapConfiguration(new R2RMLConfiguration().R2RMLMappings);
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

        [TestCase("TableName", "TableName")]
        [TestCase("[TableName]", "TableName")]
        [TestCase("[Table1Name]", "Table1Name")]
        [TestCase("'TableName'", "TableName")]
        [TestCase("`TableName`", "TableName")]
        [TestCase("`Table12Name`", "Table12Name")]
        [TestCase("\"TableName\"", "TableName")]
        public void TriplesMapTableNameShouldBeTrimmed(string tableName, string expected)
        {
            // when
            _triplesMapConfiguration.TableName = tableName;

            // then
            Assert.AreEqual(expected, _triplesMapConfiguration.TableName);
        }

        [TestCase("`Schema`.`TableName`")]
        [TestCase("[Schema].[TableName]")]
        [TestCase("[Schema].[TableName]")]
        [TestCase("Schema.[TableName]")]
        [TestCase("Schema.`TableName`")]
        public void TriplesMapTableNameCanContainSchema(string tableName)
        {
            // when
            _triplesMapConfiguration.TableName = tableName;

            // then
            Assert.AreEqual("Schema.TableName", _triplesMapConfiguration.TableName);
        }

        [TestCase("[Database].[Schema].[TableName]")]
        [TestCase("Database.[Schema].[TableName]")]
        [TestCase("`Database`.`Schema`.`TableName`")]
        [TestCase("Database.`Schema`.`TableName`")]
        public void TriplesMapTableNameCanContainSchemaAndDatabaseName(string tableName)
        {
            // when
            _triplesMapConfiguration.TableName = tableName;

            // then
            Assert.AreEqual("Database.Schema.TableName", _triplesMapConfiguration.TableName);
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullTableName(string tableName)
        {
            _triplesMapConfiguration.TableName = tableName;
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullSqlQuery(string sqlQuery)
        {
            _triplesMapConfiguration.SqlQuery = sqlQuery;
        }

        [TestCase("Table1", "Table1")]
        [TestCase("Table1", "Table2")]
        public void CannotAssignTableNameTwice(string tableName, string secondTableName)
        {
            // when
            _triplesMapConfiguration.TableName = tableName;

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _triplesMapConfiguration.TableName = secondTableName);
        }
    }
}
