using System.Linq;
using NUnit.Framework;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    public abstract class DatabaseSchemaAdapterTestsBase
    {
        protected DatabaseSchemaAdapter DatabaseSchema;

        [TestFixtureSetUp]
        public void Setup()
        {
            DatabaseSchema = SetupAdapter();
        }

        protected abstract DatabaseSchemaAdapter SetupAdapter();

        [Test]
        public void ContainsTablesCorrectly()
        {
            Assert.AreEqual(6, DatabaseSchema.Tables.Count);
            var tableNames = DatabaseSchema.Tables.Select(t => t.Name).ToArray();
            Assert.Contains("CandidateKey", tableNames);
            Assert.Contains("CandidateRef", tableNames);
            Assert.Contains("ForeignKeyReference", tableNames);
            Assert.Contains("HasPrimaryKey", tableNames);
            Assert.Contains("ManyDataTypes", tableNames);
            Assert.Contains("MultipleUniqueKeys", tableNames);
        }

        [Test]
        public void ReadsForeignKeysCorrectly()
        {
            // HasPrimaryKey - ForeignKeyReference
            TableMetadata foreignKeyReferenceTable = DatabaseSchema.Tables.Single(t => t.Name == "ForeignKeyReference");
            Assert.AreEqual(1, foreignKeyReferenceTable.ForeignKeys.Length);
            Assert.AreEqual("ForeignKey", foreignKeyReferenceTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.AreEqual("Id", foreignKeyReferenceTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.AreEqual("ForeignKeyReference", foreignKeyReferenceTable.ForeignKeys[0].TableName);
            Assert.AreEqual("HasPrimaryKey", foreignKeyReferenceTable.ForeignKeys[0].ReferencedTableName);

            // CandidateRef - CandidateKey
            TableMetadata candidateRefTable = DatabaseSchema.Tables.Single(t => t.Name == "CandidateRef");
            Assert.AreEqual(1, candidateRefTable.ForeignKeys.Length);
            Assert.AreEqual("RefCol1", candidateRefTable.ForeignKeys[0].ForeignKeyColumns[0]);
            Assert.AreEqual("RefCol2", candidateRefTable.ForeignKeys[0].ForeignKeyColumns[1]);
            Assert.AreEqual("KeyCol1", candidateRefTable.ForeignKeys[0].ReferencedColumns[0]);
            Assert.AreEqual("KeyCol2", candidateRefTable.ForeignKeys[0].ReferencedColumns[1]);
            Assert.AreEqual("CandidateRef", candidateRefTable.ForeignKeys[0].TableName);
            Assert.AreEqual("CandidateKey", candidateRefTable.ForeignKeys[0].ReferencedTableName);
        }

        [Test]
        public void ReadsPrimaryKeysCorrectly()
        {
            Assert.IsEmpty(DatabaseSchema.Tables["CandidateKey"].PrimaryKey);
            Assert.IsEmpty(DatabaseSchema.Tables["CandidateRef"].PrimaryKey);
            Assert.IsEmpty(DatabaseSchema.Tables["ManyDataTypes"].PrimaryKey);

            Assert.AreEqual(1, DatabaseSchema.Tables["HasPrimaryKey"].PrimaryKey.Length);
            Assert.AreEqual("Id", DatabaseSchema.Tables["HasPrimaryKey"].PrimaryKey[0].Name);
            Assert.AreEqual("ForeignKey", DatabaseSchema.Tables["ForeignKeyReference"].PrimaryKey[0].Name);
        }

        [Test]
        public void ReadsUniqueKeys()
        {
            Assert.AreEqual(3, DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys.Count());

            Assert.IsNotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 1 &&
                                                        uq.Any(col => col.Name == "UQ3")));
            Assert.IsNotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 2 &&
                                                        uq.Any(col => col.Name == "UQ1_1") &&
                                                        uq.Any(col => col.Name == "UQ1_2")));
            Assert.IsNotNull(DatabaseSchema.Tables["MultipleUniqueKeys"].UniqueKeys
                                 .SingleOrDefault(uq => uq.ColumnsCount == 3 &&
                                                        uq.Any(col => col.Name == "UQ2_1") &&
                                                        uq.Any(col => col.Name == "UQ2_2") &&
                                                        uq.Any(col => col.Name == "UQ2_3")));
        }
    }
}
