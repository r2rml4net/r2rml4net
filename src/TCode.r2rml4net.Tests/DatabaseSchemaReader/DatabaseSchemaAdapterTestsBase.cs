using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    public abstract class DatabaseSchemaAdapterTestsBase
    {
        DatabaseSchemaAdapter _databaseSchema;

        [SetUp]
        public void Setup()
        {
            _databaseSchema = new DatabaseSchemaAdapter(SetupAdapter());
        }

        protected abstract DatabaseReader SetupAdapter();

        [Test]
        public void ContainsCorrectNumberOfTables()
        {
            Assert.AreEqual(5, _databaseSchema.Tables.Count);
        }
    }
}
