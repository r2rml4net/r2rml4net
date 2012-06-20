using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
