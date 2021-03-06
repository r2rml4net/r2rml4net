﻿#region Licence
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
using System.Linq;
using Moq;
using Xunit;
using TCode.r2rml4net.Mapping.Direct;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.Mapping.Tests.Mocks;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class R2RMLMappingGeneratorStrategyTests
    {
        private readonly R2RMLMappingGenerator _generator;
        private readonly Mock<IDatabaseMetadata> _databaseMetedata;
        private readonly Mock<IDirectMappingStrategy> _mappingStrategy;
        private readonly Mock<IForeignKeyMappingStrategy> _foreignKeyStrategy;
        private readonly Mock<IColumnMappingStrategy> _columnStrategy;
        private readonly Mock<IR2RMLConfiguration> _configuration;
        private readonly Uri _mappingBaseUri = new Uri("http://base.uri/");

        public R2RMLMappingGeneratorStrategyTests()
        {
            _configuration = new Mock<IR2RMLConfiguration>();
            _configuration.SetupGet(conf => conf.Options).Returns(new MappingOptions());
            _databaseMetedata = new Mock<IDatabaseMetadata>();
            _mappingStrategy = new Mock<IDirectMappingStrategy>(MockBehavior.Strict);
            _foreignKeyStrategy = new Mock<IForeignKeyMappingStrategy>(MockBehavior.Strict);
            _columnStrategy = new Mock<IColumnMappingStrategy>(MockBehavior.Strict);
            _generator = new R2RMLMappingGenerator(_databaseMetedata.Object, _configuration.Object)
                {
                    MappingStrategy = _mappingStrategy.Object,
                    ColumnMappingStrategy = _columnStrategy.Object,
                    MappingBaseUri = _mappingBaseUri
                };
            _configuration.Setup(conf => conf.CreateTriplesMapFromR2RMLView(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
            _configuration.Setup(conf => conf.CreateTriplesMapFromTable(It.IsAny<string>()))
                .Returns(new MockConfiguration(_mappingBaseUri, _configuration.Object));
        }

        [Fact]
        public void TestForTableWithoutPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D003_1table3columns);
        }

        [Fact]
        public void AnotherTestForTableWithoutPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D001_1table1column);
        }

        [Fact]
        public void TestForTableWithMultipleColumns()
        {
            TestStrategyUsage(RelationalTestMappings.TypedColumns);
        }

        [Fact]
        public void TestForTableWithCompositePrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D008_1table1compositeprimarykey3columns);
        }

        [Fact]
        public void TestForTableWithPrimaryKey()
        {
            TestStrategyUsage(RelationalTestMappings.D006_1table1primarykey1column);
        }

        [Fact]
        public void TestForTableWithForeignKey()
        {
            TestStrategyUsage(RelationalTestMappings.D009_2tables1primarykey1foreignkey);
        }

        [Fact]
        public void TestForTableWithCompositeKeyReference()
        {
            TestStrategyUsage(RelationalTestMappings.D015_1table3columns1composityeprimarykey);
        }

        private void TestStrategyUsage(TableCollection tables)
        {
            // given
            _mappingStrategy.Setup(ms => ms.CreateSubjectMapForPrimaryKey(It.IsAny<ISubjectMapConfiguration>(), _mappingBaseUri, It.IsAny<TableMetadata>()));
            _mappingStrategy.Setup(ms => ms.CreateSubjectMapForNoPrimaryKey(It.IsAny<ISubjectMapConfiguration>(), _mappingBaseUri, It.IsAny<TableMetadata>()));
            _mappingStrategy.Setup(ms => ms.CreatePredicateMapForForeignKey(It.IsAny<ITermMapConfiguration>(), _mappingBaseUri, It.IsAny<ForeignKeyMetadata>()));
            _mappingStrategy.Setup(ms => ms.CreateObjectMapForPrimaryKeyReference(It.IsAny<IObjectMapConfiguration>(), _mappingBaseUri, It.IsAny<ForeignKeyMetadata>()));
            _columnStrategy.Setup(ms => ms.CreatePredicateUri(_mappingBaseUri, It.IsAny<ColumnMetadata>()))
                           .Returns(new Uri("http://predicate.uri"));
            _databaseMetedata.Setup(meta => meta.Tables).Returns(tables);

            // when
            _generator.GenerateMappings();

            // then
            foreach (var table in tables)
            {
                foreach (var column in table)
                {
                    ColumnMetadata column1 = column;
                    _columnStrategy.Verify(ms => ms.CreatePredicateUri(_mappingBaseUri, column1), Times.Once());
                }

                foreach (var fk in table.ForeignKeys)
                {
                    ForeignKeyMetadata fk1 = fk;
                    _mappingStrategy.Verify(ms => ms.CreatePredicateMapForForeignKey(It.IsAny<ITermMapConfiguration>(), _mappingBaseUri, fk1), Times.Once());
                    if (fk.IsCandidateKeyReference)
                    {
                        _foreignKeyStrategy.Verify(fks => fks.CreateObjectTemplateForCandidateKeyReference(fk1), Times.Once());
                        _mappingStrategy.Verify(ms =>
                            ms.CreateObjectMapForCandidateKeyReference(It.IsAny<IObjectMapConfiguration>(), It.IsAny<ForeignKeyMetadata>()),
                            Times.Once());
                    }
                    else
                    {
                        _mappingStrategy.Verify(ms =>
                            ms.CreateObjectMapForPrimaryKeyReference(It.IsAny<IObjectMapConfiguration>(), _mappingBaseUri, It.IsAny<ForeignKeyMetadata>()),
                            Times.Once());
                    }
                }
            }

            foreach (var table in tables.Where(t => t.PrimaryKey.Length == 0))
            {
                TableMetadata table1 = table;
                _mappingStrategy.Setup(ms =>
                    ms.CreateSubjectMapForNoPrimaryKey(
                    It.IsAny<ISubjectMapConfiguration>(),
                    _mappingBaseUri,
                    table1));

            }

            foreach (var table in tables.Where(t => t.PrimaryKey.Length > 0))
            {
                TableMetadata table1 = table;
                _mappingStrategy.Setup(ms =>
                    ms.CreateSubjectMapForPrimaryKey(
                    It.IsAny<ISubjectMapConfiguration>(),
                    _mappingBaseUri,
                    table1));
            }
        }
    }
}