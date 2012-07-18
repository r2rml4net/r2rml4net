using System;
using NUnit.Framework;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        TriplesMapConfiguration _triplesMapConfiguration;

        [SetUp]
        public void Setup()
        {
            // Initialize TriplesMapConfiguration with a default graph
            var r2RMLConfiguration = new R2RMLConfiguration();
            _triplesMapConfiguration = new TriplesMapConfiguration(r2RMLConfiguration, r2RMLConfiguration.R2RMLMappings);
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

        [Test]
        public void CannotAssignTableNameTwice()
        {
            // when
            _triplesMapConfiguration.TableName = "TABLE1";

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _triplesMapConfiguration.TableName = "TABLE2");
        }

        [Test]
        public void CannotAssignSqlQueryTwice()
        {
            // when
            _triplesMapConfiguration.SqlQuery = "SELECT * FROM X";

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _triplesMapConfiguration.SqlQuery = "SELECT * FROM Y");
        }

        [Test]
        public void CannotAssignSqlQueryHavingAlreadyAssignedTable()
        {
            // when
            _triplesMapConfiguration.TableName = "SomeTable";

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _triplesMapConfiguration.SqlQuery = "SELECT * FROM Y");
        }

        [Test]
        public void CannotAssignTableNameHavingAlreadyAssignedSqlQuery()
        {
            // when
            _triplesMapConfiguration.SqlQuery = "SELECT * FROM X";

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _triplesMapConfiguration.TableName = "SomeTable");
        }

        [Test]
        public void CanSetSqlVersionTwice()
        {
            // given
            _triplesMapConfiguration.SqlQuery = "SELECT * from X";

            // when
            _triplesMapConfiguration.SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"))
                                    .SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#MSSQL"));

            // then
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#SQL2008");
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#MSSQL");
        }

        [Test]
        public void CanSetSameSqlVersionTwice()
        {
            // given
            _triplesMapConfiguration.SqlQuery = "SELECT * from X";

            // when
            _triplesMapConfiguration.SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"))
                                    .SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"));

            // then
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#SQL2008", 2);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanSetSqlVersion(bool fromUriString)
        {
            // given
            const string sqlVersionString = "http://www.w3.org/ns/r2rml#SQL2008";
            Uri sqlVersion = new Uri(sqlVersionString);
            _triplesMapConfiguration.SqlQuery = "SELECT * from X";

            // when
            if (fromUriString)
                _triplesMapConfiguration.SetSqlVersion(sqlVersionString);
            else
                _triplesMapConfiguration.SetSqlVersion(sqlVersion);

            // then
            if (fromUriString)
                _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", sqlVersionString);
            else
                _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", sqlVersion);

            Assert.Contains(sqlVersion, _triplesMapConfiguration.SqlVersions);
        }

        [Test]
        public void CannotSetSqlVersionWhenTableNameHasAlreadyBeenSet()
        {
            // given
            _triplesMapConfiguration.TableName = "Table";

            // then
            Assert.Throws<InvalidTriplesMapException>(
                () => _triplesMapConfiguration.SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"))
            );
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CannotCreateSubjectMapBeforeInitializingTriplesMap()
        {
            var subjectMap = _triplesMapConfiguration.SubjectMap;
        }

        [Test]
        public void CanCreateSubjectMaps()
        {
            // given
            _triplesMapConfiguration.TableName = "Table";

            // when
            var subjectMapConfiguration = (ISubjectMapConfiguration) _triplesMapConfiguration.SubjectMap;

            // then
            Assert.IsNotNull(subjectMapConfiguration);
            Assert.IsInstanceOf<TermMapConfiguration>(subjectMapConfiguration);
            Assert.IsInstanceOf<ITermMapConfiguration>(subjectMapConfiguration);
        }

        [Test]
        public void SubjectMapAlwaysReturnsSameInstance()
        {
            // given 
            _triplesMapConfiguration.TableName = "Table";
            var subjectMapConfiguration = (ISubjectMapConfiguration) _triplesMapConfiguration.SubjectMap;

            // when
            var shouldBeTheSame = (ISubjectMapConfiguration) _triplesMapConfiguration.SubjectMap;

            // then
            Assert.AreSame(subjectMapConfiguration, shouldBeTheSame);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CannotCreatePropertyMapBeforeInitializingTriplesMap()
        {
            IPredicateObjectMapConfiguration predicateObjectMap = _triplesMapConfiguration.CreatePropertyObjectMap();
        }

        [Test]
        public void CanCreatePropertyObjectMap()
        {
            // given
            _triplesMapConfiguration.TableName = "Table";

            // when
            IPredicateObjectMapConfiguration predicateObjectMap = _triplesMapConfiguration.CreatePropertyObjectMap();

            // then
            Assert.IsNotNull(predicateObjectMap);
            Assert.IsInstanceOf<PredicateObjectMapConfiguration>(predicateObjectMap);
        }
    }
}
