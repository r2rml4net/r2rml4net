using System;
using NUnit.Framework;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.Metadata
{
    [TestFixture]
    public class TableCollectionTests
    {
        [Test]
        public void CanBeIndexed()
        {
            // given
            var table1 = new TableMetadata { Name = "Table1" };
            var table2 = new TableMetadata { Name = "Table2" };
            TableCollection tables = new TableCollection { table1, table2 };

            // then
            Assert.AreSame(table1, tables["Table1"]);
            Assert.AreSame(table2, tables["Table2"]);
        }

        [Test]
        public void ThrowsWhenIndexingWithAnInvalidTableName()
        {
            // given
            var table1 = new TableMetadata { Name = "Table1" };
            var table2 = new TableMetadata { Name = "Table2" };
            TableCollection tables = new TableCollection { table1, table2 };

            // then
            Assert.Throws<IndexOutOfRangeException>(() => { var table = tables["table"]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var table = tables[""]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var table = tables[" "]; });
            Assert.Throws<ArgumentNullException>(() => { var table = tables[null]; });
        }

        [Test]
        public void CanContainUniquellyNamedTables()
        {
            // given
            TableCollection tables = new TableCollection();

            // when
            tables.Add(new TableMetadata
                           {
                               Name = "Table"
                           });

            // then
            Assert.Throws<ArgumentException>(() => tables.Add(new TableMetadata { Name = "Table" }));
        }

        [Test]
        public void IsCreatedEmpty()
        {
            // given
            var collection = new TableCollection();

            // then
            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void HasCorrectCount()
        {
            // given
            var collection = new TableCollection();

            // when
            collection.Add(new TableMetadata { Name = "a" });
            collection.Add(new TableMetadata { Name = "b" });
            collection.Add(new TableMetadata { Name = "c" });

            // then
            Assert.AreEqual(3, collection.Count);
        }
    }
}
