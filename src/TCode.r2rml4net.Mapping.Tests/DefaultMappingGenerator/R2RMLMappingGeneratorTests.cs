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
using TCode.r2rml4net.Mapping.Direct;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Mapping.Tests.Mocks;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class R2RMLMappingGeneratorTests
    {
        private R2RMLMappingGenerator _generator;
        private Mock<IR2RMLConfiguration> _configuration;
        private Mock<IDatabaseMetadata> _databaseMetedata;
        private Mock<ISqlQueryBuilder> _sqlBuilder;
        private Mock<IDirectMappingStrategy> _mappingStrategy;
        private readonly Uri _mappingBaseUri = new Uri("http://base.uri/");

        public R2RMLMappingGeneratorTests()
        {
            _configuration = new Mock<IR2RMLConfiguration>();
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _sqlBuilder = new Mock<ISqlQueryBuilder>();
            _mappingStrategy = new Mock<IDirectMappingStrategy>();
            _generator = new R2RMLMappingGenerator(_databaseMetedata.Object, _configuration.Object)
            {
                MappingBaseUri = _mappingBaseUri,
                SqlBuilder = _sqlBuilder.Object,
                MappingStrategy = _mappingStrategy.Object
            };
            _configuration.Setup(c => c.CreateTriplesMapFromTable(It.IsAny<string>()))
                          .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
            _configuration.Setup(c => c.CreateTriplesMapFromR2RMLView(It.IsAny<string>()))
                          .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
        }

        [Fact]
        public void CreatesTriplesMapFromTableWithPrimaryKey()
        {
            //given
            const string tableName = "Country Info";
            var table = RelationalTestMappings.D010_1table1primarykey3colums[tableName];

            // when
            _generator.Visit(table);

            // then
            _configuration.Verify(conf => conf.CreateTriplesMapFromTable(tableName), Times.Once());
        }

        [Fact]
        public void CreatesTriplesMapFromTableWithoutPrimaryKey()
        {
            //given
            const string tableName = "Student";
            var table = RelationalTestMappings.D001_1table1column[tableName];

            // when
            _generator.Visit(table);

            // then
            _configuration.Verify(conf => conf.CreateTriplesMapFromTable(tableName), Times.Once());
        }

        [Fact]
        public void CreatesTriplesMapFromTableWithCandidateKeyReference()
        {
            //given
            const string tableName = "EMP";
            var table = RelationalTestMappings.D014_3tables1primarykey1foreignkey[tableName];

            // when
            _generator.Visit(table);

            // then
            _configuration.Verify(conf => conf.CreateTriplesMapFromTable(tableName), Times.Once());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreatesTriplesMapFromSqlViewForTableWithCandidateKeyReferenceReferencingTablePrimaryKey(bool isCandidateKeyRef)
        {
            //given
            const string expectedQuery = @"SELECT something";
            var referencedTable = new TableMetadata
                {
                    new ColumnMetadata {Name = "Id", IsPrimaryKey = true},
                    new ColumnMetadata {Name = "Id2", IsPrimaryKey = true},
                    new ColumnMetadata {Name = "unique1"},
                    new ColumnMetadata {Name = "unique2"}
                };
            referencedTable.Name = "Referenced";
            var table = new TableMetadata
                {
                    ForeignKeys = new[]
                        {
                            new ForeignKeyMetadata
                                {
                                    TableName = "Referencing",
                                    IsCandidateKeyReference = isCandidateKeyRef,
                                    ReferencedTableHasPrimaryKey = true,
                                    ReferencedColumns = new[] {"unique1", "unique2"},
                                    ForeignKeyColumns = new[] {"fk1", "fk2"},
                                    ReferencedTable = referencedTable,
                                }
                        }
                };
            table.Name = "TableName";
            _sqlBuilder.Setup(sql => sql.GetR2RMLViewForJoinedTables(table)).Returns(expectedQuery);

            // when
            _generator.Visit(table);

            // then
            if (isCandidateKeyRef)
            {
                _sqlBuilder.Verify(sql => sql.GetR2RMLViewForJoinedTables(table), Times.Once()); 
                _configuration.Verify(conf => conf.CreateTriplesMapFromR2RMLView(expectedQuery), Times.Once());
            }
            else
                _configuration.Verify(conf => conf.CreateTriplesMapFromTable("TableName"), Times.Once());
        }

        [Fact]
        public void CreatesUriTemplateIfCandidateForeignKeyTargetHasPrimaryKey()
        {
            // given 
            TableMetadata referenced = new TableMetadata
                {
                    new ColumnMetadata { Name = "Id" }
                };
            referenced.Name = "target";
            var fk = new ForeignKeyMetadata
                {
                    ReferencedColumns = new[] { "unique1", "unique2" },
                    ForeignKeyColumns = new[] { "candidate1", "candidate2" },
                    IsCandidateKeyReference = true,
                    ReferencedTableHasPrimaryKey = true,
                    TableName = "source",
                    ReferencedTable = referenced
                };
            _generator.CurrentTriplesMapConfiguration = new MockConfiguration(_mappingBaseUri, _configuration.Object);

            // when
            _generator.Visit(fk);

            // then
            _mappingStrategy.Verify(ms =>
                                    ms.CreateObjectMapForPrimaryKeyReference(
                                        It.IsAny<IObjectMapConfiguration>(),
                                        _mappingBaseUri,
                                        fk),
                                    Times.Once());
        }
    }
}