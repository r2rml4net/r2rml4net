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
            string[] primaryKey = table.PrimaryKey;

            // then
            Assert.AreEqual(1, primaryKey.Length);
            Assert.Contains(primaryKeyColumn.Name, primaryKey);
        }

        [Test]
        public void ReturnsCompositePrimaryKey()
        {
            // given
            var primaryKeyColumn1 = new ColumnMetadata { Name = "OtherColumn", IsPrimaryKey = true };
            var primaryKeyColumn2 = new ColumnMetadata { Name = "OtherColumn2", IsPrimaryKey = true };
            var primaryKeyColumn3 = new ColumnMetadata { Name = "OtherColumn3", IsPrimaryKey = true };
            TableMetadata table = new TableMetadata
                                      {
                                          primaryKeyColumn1,
                                          primaryKeyColumn2,
                                          primaryKeyColumn3
                                      };

            // when
            string[] primaryKey = table.PrimaryKey;

            // then
            Assert.AreEqual(3, primaryKey.Length);
            Assert.Contains(primaryKeyColumn1.Name, primaryKey);
            Assert.Contains(primaryKeyColumn2.Name, primaryKey);
            Assert.Contains(primaryKeyColumn3.Name, primaryKey);
        }

        [Test]
        public void CanBeIndexed()
        {
            // given
            var column1 = new ColumnMetadata { Name = "Column1" };
            var column2 = new ColumnMetadata { Name = "Column2" };
            var table = new TableMetadata
                             {
                                 column1,
                                 column2
                             };

            // then
            Assert.AreSame(column1, table["Column1"]);
            Assert.AreSame(column2, table["Column2"]);
        }

        [Test]
        public void ThrowsWhenIndexingWithAnInvalidColumnName()
        {
            // given
            var column1 = new ColumnMetadata { Name = "Column1" };
            var column2 = new ColumnMetadata { Name = "Column2" };
            var table = new TableMetadata
                             {
                                 column1,
                                 column2
                             };

            // then
            Assert.Throws<IndexOutOfRangeException>(() => { var column = table["column"]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var column = table[""]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var column = table[" "]; });
            Assert.Throws<ArgumentNullException>(() => { var column = table[null]; });
        }

        [Test]
        public void CanContainUniquellyNamedColumns()
        {
            // given
            TableMetadata table = new TableMetadata();

            // when
            table.Add(new ColumnMetadata
            {
                Name = "Column"
            });

            // then
            Assert.Throws<ArgumentException>(() => table.Add(new ColumnMetadata { Name = "Column" }));
        }

        [Test]
        public void TableWithNoForeignKeysReturnsEmptyCollection()
        {
            // given
            TableMetadata table = new TableMetadata
                {
                    new ColumnMetadata{Name="Id", IsPrimaryKey=true},
                    new ColumnMetadata{Name="A"},
                    new ColumnMetadata{Name="B"}
                };

            // when
            var foreignKeys = table.ForeignKeys;

            // then
            Assert.IsNotNull(foreignKeys);
            Assert.IsEmpty(foreignKeys);
        }
    }
}
