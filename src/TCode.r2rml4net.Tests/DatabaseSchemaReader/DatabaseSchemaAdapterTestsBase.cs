using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    public abstract class DatabaseSchemaAdapterTestsBase
    {
        DatabaseSchemaAdapter _databaseSchema;

        [TestFixtureSetUp]
        public void Setup()
        {
            _databaseSchema = new DatabaseSchemaAdapter(SetupAdapter());
        }

        protected abstract DatabaseReader SetupAdapter();

        [Test]
        public void ContainsTablesCorrectly()
        {
            Assert.AreEqual(5, _databaseSchema.Tables.Count);
            var tableNames = _databaseSchema.Tables.Select(t => t.Name).ToArray();
            Assert.Contains("CandidateKey", tableNames);
            Assert.Contains("CandidateRef", tableNames);
            Assert.Contains("ForeignKeyReference", tableNames);
            Assert.Contains("HasPrimaryKey", tableNames);
            Assert.Contains("ManyDataTypes", tableNames);
        }

        [Test]
        public void ReadsForeignKeysCorrectly()
        {
            // HasPrimaryKey - ForeignKeyReference
            TableMetadata ForeignKeyReferenceTable = _databaseSchema.Tables.Single(t => t.Name == "ForeignKeyReference");
            Assert.AreEqual(1, ForeignKeyReferenceTable.ForeignKeys.Length);
            Assert.AreEqual("ForeignKey", ForeignKeyReferenceTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.AreEqual("Id", ForeignKeyReferenceTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.AreEqual("ForeignKeyReference", ForeignKeyReferenceTable.ForeignKeys[0].TableName);
            Assert.AreEqual("HasPrimaryKey", ForeignKeyReferenceTable.ForeignKeys[0].ReferencedTableName);

            // CandidateRef - CandidateKey
            TableMetadata CandidateRefTable = _databaseSchema.Tables.Single(t => t.Name == "CandidateRef");
            Assert.AreEqual(1, CandidateRefTable.ForeignKeys.Length);
            Assert.AreEqual("RefCol1", CandidateRefTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.AreEqual("RefCol2", CandidateRefTable.ForeignKeys[0].ForeignKeyColumns[1]);
            Assert.AreEqual("KeyCol1", CandidateRefTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.AreEqual("KeyCol2", CandidateRefTable.ForeignKeys[0].ReferencedColumns[1]);
            Assert.AreEqual("CandidateRef", CandidateRefTable.ForeignKeys[0].TableName);
            Assert.AreEqual("CandidateKey", CandidateRefTable.ForeignKeys[0].ReferencedTableName);
        }

        [Test]
        public void ReadsPrimaryKeysCorrectly()
        {
            Assert.IsEmpty(_databaseSchema.Tables["CandidateKey"].PrimaryKey);
            Assert.IsEmpty(_databaseSchema.Tables["CandidateRef"].PrimaryKey);
            Assert.IsEmpty(_databaseSchema.Tables["ManyDataTypes"].PrimaryKey);

            Assert.AreEqual(1, _databaseSchema.Tables["HasPrimaryKey"].PrimaryKey.Length);
            Assert.AreEqual("Id", _databaseSchema.Tables["HasPrimaryKey"].PrimaryKey[0].Name);
            Assert.AreEqual("ForeignKey", _databaseSchema.Tables["ForeignKeyReference"].PrimaryKey[0].Name);
        }
    }
}
