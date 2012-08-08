using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDB;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class TriplesMapConfigurationTests
    {
        TriplesMapConfiguration _triplesMapConfiguration;
        private Mock<IR2RMLConfiguration> _r2RMLConfiguration;
        private IGraph _r2RMLMappings;

        [SetUp]
        public void Setup()
        {
            // Initialize TriplesMapConfiguration with a default graph
            var r2RMLConfiguration = new R2RMLConfiguration();
            _r2RMLConfiguration = new Mock<IR2RMLConfiguration>();
            _r2RMLMappings = r2RMLConfiguration.R2RMLMappings;
            _triplesMapConfiguration = new TriplesMapConfiguration(_r2RMLConfiguration.Object, _r2RMLMappings, _r2RMLMappings.CreateBlankNode());
        }

        [Test]
        public void SqlQueryIsSetAsIs()
        {
            // given
            const string query = "SELECT * from Table";

            // when
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(_r2RMLConfiguration.Object, _r2RMLMappings, query);

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, tableName);

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, tableName);

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, tableName);

            // then
            Assert.AreEqual("Database.Schema.TableName", _triplesMapConfiguration.TableName);
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullTableName(string tableName)
        {
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, tableName);
        }

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullSqlQuery(string sqlQuery)
        {
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(_r2RMLConfiguration.Object, _r2RMLMappings, sqlQuery);
        }

        [Test]
        public void CanSetSqlVersionTwice()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(_r2RMLConfiguration.Object, _r2RMLMappings, "SELECT * from X");

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(_r2RMLConfiguration.Object, _r2RMLMappings, "SELECT * from X");

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(_r2RMLConfiguration.Object, _r2RMLMappings, "SELECT * from X");

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, "Table");

            // then
            Assert.Throws<InvalidTriplesMapException>(
                () => _triplesMapConfiguration.SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"))
            );
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CannotGetSubjectMapBeforeInitializingTriplesMap()
        {
            var subjectMap = _triplesMapConfiguration.SubjectMap;
        }

        [Test]
        public void CanCreateSubjectMaps()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, "Table");

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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, "Table");
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
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(_r2RMLConfiguration.Object, _r2RMLMappings, "Table");

            // when
            IPredicateObjectMapConfiguration predicateObjectMap = _triplesMapConfiguration.CreatePropertyObjectMap();

            // then
            Assert.IsNotNull(predicateObjectMap);
            Assert.IsInstanceOf<PredicateObjectMapConfiguration>(predicateObjectMap);
        }

        [Test]
        public void UsesEffectiveSqlBuilder()
        {
            // given
            const string excpetedSql = "SELECT * FROM (SELECT * FROM A) AS tmp";
            Mock<ISqlQueryBuilder> sqlBuilder = new Mock<ISqlQueryBuilder>();
            sqlBuilder.Setup(builder => builder.GetEffectiveQueryForTriplesMap(It.IsAny<ITriplesMap>()))
                      .Returns(excpetedSql);
            _r2RMLConfiguration.Setup(config => config.SqlQueryBuilder).Returns(sqlBuilder.Object);

            // when
            string sql = _triplesMapConfiguration.EffectiveSqlQuery;

            // then
            Assert.AreEqual(excpetedSql, sql);
        }
    }
}
