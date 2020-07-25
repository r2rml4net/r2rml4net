#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
//
// ------------------------------------------------------------------------
//
// This file is part of r2rml4net.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.
//
// ------------------------------------------------------------------------
//
// r2rml4net may alternatively be used under the LGPL licence
//
// http://www.gnu.org/licenses/lgpl.html
//
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class TriplesMapConfigurationTests
    {
        private readonly Mock<ISqlVersionValidator> _sqlVersionValidator;
        TriplesMapConfiguration _triplesMapConfiguration;
        private readonly Mock<IR2RMLConfiguration> _r2RMLConfiguration;
        private readonly IGraph _r2RMLMappings;
        private Mock<ISqlVersionValidator> _sqlValidator;

        private TriplesMapConfigurationStub CreatStub()
        {
            _sqlValidator = new Mock<ISqlVersionValidator>();
            _sqlValidator.Setup(v => v.SqlVersionIsValid(It.IsAny<Uri>())).Returns(true);
            _r2RMLConfiguration.Setup(config => config.SqlVersionValidator).Returns(_sqlValidator.Object);
            return new TriplesMapConfigurationStub(_r2RMLConfiguration.Object, _r2RMLMappings, _sqlVersionValidator.Object);
        }

        public TriplesMapConfigurationTests()
        {
            // Initialize TriplesMapConfiguration with a default graph
            var r2RMLConfiguration = new FluentR2RML(new MappingOptions());
            _r2RMLConfiguration = new Mock<IR2RMLConfiguration>();
            _r2RMLMappings = r2RMLConfiguration.R2RMLMappings;
            _sqlVersionValidator = new Mock<ISqlVersionValidator>(MockBehavior.Strict);
            _triplesMapConfiguration = new TriplesMapConfiguration(CreatStub(), _r2RMLMappings.CreateBlankNode(), new MappingOptions());
        }

        [Fact]
        public void SqlQueryIsSetAsIs()
        {
            // given
            const string query = "SELECT * from Table";

            // when
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(CreatStub(), query, new MappingOptions());

            // then
            Assert.Equal(query, _triplesMapConfiguration.SqlQuery);
            Assert.Null(_triplesMapConfiguration.TableName);
        }

        [Theory]
        [InlineData("TableName", "TableName")]
        [InlineData("[TableName]", "TableName")]
        [InlineData("[Table1Name]", "Table1Name")]
        [InlineData("'TableName'", "TableName")]
        [InlineData("`TableName`", "TableName")]
        [InlineData("`Table12Name`", "Table12Name")]
        [InlineData("\"TableName\"", "TableName")]
        public void TriplesMapTableNameShouldBeTrimmed(string tableName, string expected)
        {
            // when
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), tableName, new MappingOptions());

            // then
            Assert.Equal(expected, _triplesMapConfiguration.TableName);
            Assert.Null(_triplesMapConfiguration.SqlQuery);
        }

        [Theory]
        [InlineData("`Schema`.`TableName`")]
        [InlineData("[Schema].[TableName]")]
        [InlineData("Schema.[TableName]")]
        [InlineData("Schema.`TableName`")]
        public void TriplesMapTableNameCanContainSchema(string tableName)
        {
            // when
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), tableName, new MappingOptions());

            // then
            Assert.Equal("Schema.TableName", _triplesMapConfiguration.TableName);
            Assert.Null(_triplesMapConfiguration.SqlQuery);
        }

        [Theory]
        [InlineData("[Database].[Schema].[TableName]")]
        [InlineData("Database.[Schema].[TableName]")]
        [InlineData("`Database`.`Schema`.`TableName`")]
        [InlineData("Database.`Schema`.`TableName`")]
        public void TriplesMapTableNameCanContainSchemaAndDatabaseName(string tableName)
        {
            // when
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), tableName, new MappingOptions());

            // then
            Assert.Equal("Database.Schema.TableName", _triplesMapConfiguration.TableName);
            Assert.Null(_triplesMapConfiguration.SqlQuery);
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("", typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullTableName(string tableName, Type exceptionType)
        {
            Assert.Throws(exceptionType, () =>
                _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), tableName, new MappingOptions())
            );
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("", typeof(ArgumentOutOfRangeException))]
        public void CannotCreateTriplesMapFromEmptyOrNullSqlQuery(string sqlQuery, Type exceptionType)
        {
            Assert.Throws(exceptionType, () =>
                _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(CreatStub(), sqlQuery, new MappingOptions())
            );
        }

        [Fact]
        public void CanSetSqlVersionTwice()
        {
            // given
            var mssql = new Uri("http://www.w3.org/ns/r2rml#MSSQL");
            var coreSql2008 = new Uri("http://www.w3.org/ns/r2rml#SQL2008");
            _sqlVersionValidator.Setup(val => val.SqlVersionIsValid(coreSql2008)).Returns(true);
            _sqlVersionValidator.Setup(val => val.SqlVersionIsValid(mssql)).Returns(true);
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(CreatStub(), "SELECT * from X", new MappingOptions());

            // when
            _triplesMapConfiguration.SetSqlVersion(coreSql2008)
                                    .SetSqlVersion(mssql);

            // then
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#SQL2008");
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#MSSQL");
        }

        [Fact]
        public void CanSetSameSqlVersionTwice()
        {
            // given
            var sqlVersion = new Uri("http://www.w3.org/ns/r2rml#SQL2008");
            _sqlVersionValidator.Setup(val => val.SqlVersionIsValid(sqlVersion)).Returns(true);
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(CreatStub(), "SELECT * from X", new MappingOptions());

            // when
            _triplesMapConfiguration.SetSqlVersion(sqlVersion)
                                    .SetSqlVersion(sqlVersion);

            // then
            _triplesMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject("http://www.w3.org/ns/r2rml#sqlVersion", "http://www.w3.org/ns/r2rml#SQL2008", 2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanSetSqlVersion(bool fromUriString)
        {
            // given
            const string sqlVersionString = "http://www.w3.org/ns/r2rml#SQL2008";
            Uri sqlVersion = new Uri(sqlVersionString);
            _sqlVersionValidator.Setup(val => val.SqlVersionIsValid(sqlVersion)).Returns(true);
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(CreatStub(), "SELECT * from X", new MappingOptions());

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

        [Fact]
        public void CannotSetSqlVersionWhenTableNameHasAlreadyBeenSet()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), "Table", new MappingOptions());

            // then
            Assert.Throws<InvalidMapException>(
                () => _triplesMapConfiguration.SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"))
            );
        }

        [Fact]
        public void ByDefaultCannotSetInvalidSqlVersion()
        {
            // given
            var sqlVersion = new Uri("http://no-such-identifier.com");
            _sqlVersionValidator.Setup(v => v.SqlVersionIsValid(sqlVersion)).Returns(false);
            TriplesMapConfigurationStub stub = new TriplesMapConfigurationStub(_r2RMLConfiguration.Object,
                                                                               _r2RMLMappings,
                                                                               _sqlVersionValidator.Object);
            _r2RMLConfiguration.Setup(config => config.SqlVersionValidator).Returns(_sqlVersionValidator.Object);
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(stub, "SELECT * FROM Table", new MappingOptions());

            // when
            Assert.Throws<InvalidSqlVersionException>(() => _triplesMapConfiguration.SetSqlVersion(sqlVersion));

            // then
            _sqlVersionValidator.Verify(v => v.SqlVersionIsValid(sqlVersion), Times.Once());
        }

        [Fact]
        public void CanUseSettingToDisableSqlVersionValidation()
        {
            // given
            var sqlVersion = new Uri("http://no-such-identifier.com");
            var options = new MappingOptions().WithSqlVersionValidation(false);
            var stub = new TriplesMapConfigurationStub(_r2RMLConfiguration.Object,
                                                                               _r2RMLMappings,
                                                                               _sqlVersionValidator.Object);
            _triplesMapConfiguration = TriplesMapConfiguration.FromSqlQuery(stub, "SELECT * FROM Table", options);

            // when
            _triplesMapConfiguration.SetSqlVersion(sqlVersion);

            // then
            Assert.Contains(sqlVersion, _triplesMapConfiguration.SqlVersions);
        }

        [Fact]
        public void CanCreateSubjectMaps()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), "Table", new MappingOptions());

            // when
            var subjectMapConfiguration = (ISubjectMapConfiguration)_triplesMapConfiguration.SubjectMap;

            // then
            Assert.NotNull(subjectMapConfiguration);
            Assert.True(subjectMapConfiguration is TermMapConfiguration);
            Assert.True(subjectMapConfiguration is ITermMapConfiguration);
        }

        [Fact]
        public void SubjectMapAlwaysReturnsSameInstance()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), "Table", new MappingOptions());
            var subjectMapConfiguration = (ISubjectMapConfiguration)_triplesMapConfiguration.SubjectMap;

            // when
            var shouldBeTheSame = (ISubjectMapConfiguration)_triplesMapConfiguration.SubjectMap;

            // then
            Assert.Same(subjectMapConfiguration, shouldBeTheSame);
        }

        [Fact]
        public void CanCreatePropertyObjectMap()
        {
            // given
            _triplesMapConfiguration = TriplesMapConfiguration.FromTable(CreatStub(), "Table", new MappingOptions());

            // when
            IPredicateObjectMapConfiguration predicateObjectMap = _triplesMapConfiguration.CreatePropertyObjectMap();

            // then
            Assert.NotNull(predicateObjectMap);
            Assert.True(predicateObjectMap is PredicateObjectMapConfiguration);
        }

        [Fact]
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
            Assert.Equal(excpetedSql, sql);
        }

        [Fact]
        public void BlankNodeTriplesMapsDontInterfereWithEachOther()
        {
            // given
            var fromTable = TriplesMapConfiguration.FromTable(CreatStub(), "Table", new MappingOptions());
            var fromSqlQuery = TriplesMapConfiguration.FromSqlQuery(CreatStub(), "SElECT * FROM y", new MappingOptions());

            // then
            Assert.Null(fromTable.SqlQuery);
            Assert.Null(fromSqlQuery.TableName);
            Assert.Equal("Table", fromTable.TableName);
            Assert.Equal("SElECT * FROM y", fromSqlQuery.SqlQuery);
        }
    }
}
