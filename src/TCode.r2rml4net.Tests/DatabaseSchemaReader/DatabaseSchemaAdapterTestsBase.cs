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
using System.Linq;
using Xunit;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    public abstract class DatabaseSchemaAdapterTestsBase
    {
        protected readonly DatabaseSchemaAdapter DatabaseSchema;

        protected DatabaseSchemaAdapterTestsBase(SqlServerFixture fixture)
        {
            DatabaseSchema = fixture.DatabaseSchema;
        }

        [Fact]
        public void ContainsTablesCorrectly()
        {
            Assert.Equal(10, DatabaseSchema.Tables.Count);
            var tableNames = DatabaseSchema.Tables.Select(t => t.Name).ToArray();
            Assert.Contains("CandidateKey", tableNames);
            Assert.Contains("CandidateRef", tableNames);
            Assert.Contains("ForeignKeyReference", tableNames);
            Assert.Contains("HasPrimaryKey", tableNames);
            Assert.Contains("ManyDataTypes", tableNames);
            Assert.Contains("MultipleUniqueKeys", tableNames);
            Assert.Contains("PrimaryAndUnique", tableNames);
            Assert.Contains("ReferencesUnique", tableNames);
        }

        [Fact]
        public void ReadsForeignKeysCorrectly()
        {
            // HasPrimaryKey - ForeignKeyReference
            TableMetadata foreignKeyReferenceTable = DatabaseSchema.Tables.Single(t => t.Name == "ForeignKeyReference");
            TableMetadata hasPrimaryKey = DatabaseSchema.Tables["HasPrimaryKey"];
            Assert.Single(foreignKeyReferenceTable.ForeignKeys);
            Assert.Equal("ForeignKey", foreignKeyReferenceTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.Equal("Id", foreignKeyReferenceTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.Equal("ForeignKeyReference", foreignKeyReferenceTable.ForeignKeys[0].TableName);
            Assert.Same(hasPrimaryKey, foreignKeyReferenceTable.ForeignKeys[0].ReferencedTable);

            // CandidateRef - CandidateKey
            TableMetadata candidateRefTable = DatabaseSchema.Tables.Single(t => t.Name == "CandidateRef");
            TableMetadata candidateKeyTable = DatabaseSchema.Tables["CandidateKey"];
            Assert.Single(candidateRefTable.ForeignKeys);
            Assert.Equal("RefCol1", candidateRefTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.Equal("RefCol2", candidateRefTable.ForeignKeys[0].ForeignKeyColumns[1]);
            Assert.Equal("KeyCol1", candidateRefTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.Equal("KeyCol2", candidateRefTable.ForeignKeys[0].ReferencedColumns[1]);
            Assert.Equal("CandidateRef", candidateRefTable.ForeignKeys[0].TableName);
            Assert.Same(candidateKeyTable, candidateRefTable.ForeignKeys[0].ReferencedTable);
            Assert.True(candidateKeyTable.UniqueKeys.ElementAt(0).IsReferenced);
        }

        [Fact]
        public void ReadsPrimaryKeysCorrectly()
        {
            Assert.Empty(DatabaseSchema.Tables["CandidateKey"].PrimaryKey);
            Assert.Empty(DatabaseSchema.Tables["CandidateRef"].PrimaryKey);
            Assert.Empty(DatabaseSchema.Tables["ManyDataTypes"].PrimaryKey);

            Assert.Single(DatabaseSchema.Tables["HasPrimaryKey"].PrimaryKey);
            Assert.Equal("Id", DatabaseSchema.Tables["HasPrimaryKey"].PrimaryKey[0]);
            Assert.Equal("ForeignKey", DatabaseSchema.Tables["ForeignKeyReference"].PrimaryKey[0]);
        }

        [Fact]
        public void ReadsUniqueKeysAndMarksReferenced()
        {
            Assert.Equal(3, DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys.Count());

            Assert.NotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 1 &&
                                                        uq.Any(col => col.Name == "UQ3") &&
                                                        uq.IsReferenced));
            Assert.NotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 2 &&
                                                        uq.Any(col => col.Name == "UQ1_1") &&
                                                        uq.Any(col => col.Name == "UQ1_2") &&
                                                        uq.IsReferenced));
            Assert.NotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 3 &&
                                                        uq.Any(col => col.Name == "UQ2_1") &&
                                                        uq.Any(col => col.Name == "UQ2_2") &&
                                                        uq.Any(col => col.Name == "UQ2_3") &&
                                                        !uq.IsReferenced));
        }

        [Fact]
        public void SetsFlagIfCandidateReferenceTargetHasPrimaryKey()
        {
            Assert.Single(DatabaseSchema.Tables["ReferencesUnique"].ForeignKeys);
            Assert.True(DatabaseSchema.Tables["ReferencesUnique"].ForeignKeys[0].IsCandidateKeyReference);
            Assert.True(DatabaseSchema.Tables["ReferencesUnique"].ForeignKeys[0].ReferencedTableHasPrimaryKey);
        }
    }
}
