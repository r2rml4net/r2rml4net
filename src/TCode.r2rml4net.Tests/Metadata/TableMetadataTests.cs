using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.Metadata
{
    [TestFixture]
    public class TableMetadataTests
    {
        [Test]
        public void ReturnsSinglePrimaryKey()
        {
            // given
            var primaryKeyColumn = new ColumnMetadata { Name = "OtherColumn", IsPrimaryKey = true };
            TableMetadata table = new TableMetadata
                                      {
                                          new ColumnMetadata{Name = "Column1"},
                                          primaryKeyColumn,
                                          new ColumnMetadata{Name = "YetAnotherColumn"}
                                      };

            // when
            ColumnMetadata[] primaryKey = table.PrimaryKey;

            // then
            Assert.AreEqual(1, primaryKey.Length);
            Assert.Contains(primaryKeyColumn, primaryKey);
        }

        [Test]
        public void ReturnsCompositePrimaryKey()
        {
            // given
            var primaryKeyColumn1 = new ColumnMetadata { Name = "OtherColumn", IsPrimaryKey = true };
            var primaryKeyColumn2 = new ColumnMetadata { Name = "OtherColumn", IsPrimaryKey = true };
            var primaryKeyColumn3 = new ColumnMetadata { Name = "OtherColumn", IsPrimaryKey = true };
            TableMetadata table = new TableMetadata
                                      {
                                          primaryKeyColumn1,
                                          primaryKeyColumn2,
                                          primaryKeyColumn3
                                      };

            // when
            ColumnMetadata[] primaryKey = table.PrimaryKey;

            // then
            Assert.AreEqual(3, primaryKey.Length);
            Assert.Contains(primaryKeyColumn1, primaryKey);
            Assert.Contains(primaryKeyColumn2, primaryKey);
            Assert.Contains(primaryKeyColumn3, primaryKey);
        }
    }
}
