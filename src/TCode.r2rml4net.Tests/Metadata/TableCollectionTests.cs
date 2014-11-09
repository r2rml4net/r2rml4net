#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
