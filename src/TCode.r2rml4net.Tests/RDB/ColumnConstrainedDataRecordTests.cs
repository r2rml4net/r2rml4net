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
using System.Data;
using System.Data.Common;
using Moq;
using Xunit;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.RDB
{
    public class ColumnConstrainedDataRecordTests
    {
        private ColumnConstrainedDataRecord _dataRecord;
        private Mock<DbDataRecord> _wrappedRecord;

        public ColumnConstrainedDataRecordTests()
        {
            _wrappedRecord = new Mock<DbDataRecord>();
            _wrappedRecord.Setup(r => r.FieldCount).Returns(10);
        }

        [Theory]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 1, 5, 1)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 1, 5, 0)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 5, 5, 5)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 5, 5, 0)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 8, 5, 5)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 8, 5, 3)]
        public void ChangesReturnedColumnCount(ColumnConstrainedDataRecord.ColumnLimitType limitType, int rows, int limit, int expectedCoulmnsCount)
        {
            // given 
            _wrappedRecord = new Mock<DbDataRecord>();
            _wrappedRecord.Setup(r => r.FieldCount).Returns(rows);
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, limit, limitType);

            // when
            int columnsCount = _dataRecord.FieldCount;

            // then
            Assert.Equal(expectedCoulmnsCount, columnsCount);
        }

        [Theory]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 0, 0)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 4, 4)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 0, 5)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 4, 9)]
        public void CallsWrappedDataRecordForValidColumnIndex(ColumnConstrainedDataRecord.ColumnLimitType limitType, int columnToFetch, int underlyingColumnFetched)
        {
            // given 
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, 5, limitType);

            // when
            _dataRecord.CheckFieldCountAndCallBase(columnToFetch, _wrappedRecord.Object.GetString);

            // then
            _wrappedRecord.Verify(r => r.GetString(underlyingColumnFetched));
        }

        [Theory]
        [InlineData(2, 2)]
        public void ReturnsCorrectOrdinalForColumnNameInFirstNColumns(int actualColumnIndex, int expectedColumnIndex)
        {
            // given
            const string columnName = "column";
            _wrappedRecord.Setup(r => r.GetOrdinal(columnName)).Returns(actualColumnIndex);
            _wrappedRecord.Setup(r => r.GetName(6)).Returns("column");
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, 5, ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns);

            // when
            var columnIndex = _dataRecord.GetOrdinal(columnName);

            // then
            _wrappedRecord.Verify(r => r.GetOrdinal(columnName), Times.Once());
            Assert.Equal(expectedColumnIndex, columnIndex);
        }

        [Theory]
        [InlineData(6, 1)]
        public void ReturnsCorrectOrdinalForColumnNameInAllButFirstNColumns(int actualColumnIndex, int expectedColumnIndex)
        {
            // given
            const string columnName = "column";
            _wrappedRecord.Setup(r => r.GetOrdinal(columnName)).Returns(actualColumnIndex);
            _wrappedRecord.Setup(r => r.GetName(actualColumnIndex)).Returns("column");
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, 5, ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns);

            // when
            var columnIndex = _dataRecord.GetOrdinal(columnName);

            // then
            _wrappedRecord.Verify(r => r.GetName(It.IsAny<int>()), Times.Exactly(2));
            Assert.Equal(expectedColumnIndex, columnIndex);
        }

        [Theory]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 5)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 9)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 0)]
        [InlineData(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 4)]
        public void ThrowsIfColumnFoundOutsideLimit(ColumnConstrainedDataRecord.ColumnLimitType limitType, int actualColumnIndex)
        {
            // given
            const string columnName = "column";
            _wrappedRecord.Setup(r => r.GetOrdinal(columnName)).Returns(actualColumnIndex);
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, 5, limitType);

            // then
            Assert.Throws<IndexOutOfRangeException>(() => _dataRecord.GetOrdinal(columnName));
        }
    }
}