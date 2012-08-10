using System;
using System.Data;
using System.Data.Common;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.RDB
{
    [TestFixture]
    public class ColumnConstrainedDataRecordTests
    {
        private ColumnConstrainedDataRecord _dataRecord;
        private Mock<DbDataRecord> _wrappedRecord;

        [SetUp]
        public void Setup()
        {
            _wrappedRecord = new Mock<DbDataRecord>();
            _wrappedRecord.Setup(r => r.FieldCount).Returns(10);
        }

        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 1, 5, 1)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 1, 5, 0)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 5, 5, 5)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 5, 5, 0)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 8, 5, 5)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 8, 5, 3)]
        public void ChangesReturnedColumnCount(ColumnConstrainedDataRecord.ColumnLimitType limitType, int rows, int limit, int expectedCoulmnsCount)
        {
            // given 
            _wrappedRecord = new Mock<DbDataRecord>();
            _wrappedRecord.Setup(r => r.FieldCount).Returns(rows);
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, limit, limitType);

            // when
            int columnsCount = _dataRecord.FieldCount;

            // then
            Assert.AreEqual(expectedCoulmnsCount, columnsCount);
        }

        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 0, 0)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 4, 4)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 0, 5)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 4, 9)]
        public void CallsWrappedDataRecordForValidColumnIndex(ColumnConstrainedDataRecord.ColumnLimitType limitType, int columnToFetch, int underlyingColumnFetched)
        {
            // given 
            _dataRecord = new ColumnConstrainedDataRecord(_wrappedRecord.Object, 5, limitType);

            // when
            _dataRecord.CheckFieldCountAndCallBase(columnToFetch, _wrappedRecord.Object.GetString);

            // then
            _wrappedRecord.Verify(r => r.GetString(underlyingColumnFetched));
        }

        [TestCase(2, 2)]
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
            Assert.AreEqual(expectedColumnIndex, columnIndex);
        }

        [TestCase(6, 1)]
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
            Assert.AreEqual(expectedColumnIndex, columnIndex);
        }

        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 5)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.FirstNColumns, 9)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 0)]
        [TestCase(ColumnConstrainedDataRecord.ColumnLimitType.AllButFirstNColumns, 4)]
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