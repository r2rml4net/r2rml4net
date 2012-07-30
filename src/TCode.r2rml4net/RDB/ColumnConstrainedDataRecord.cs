using System;
using System.Data;

namespace TCode.r2rml4net.RDB
{
    public class ColumnConstrainedDataRecord : IDataRecord
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnLimit;
        private readonly ColumnLimitType _limitType;

        public ColumnConstrainedDataRecord(IDataRecord dataRecord, int columnLimit, ColumnLimitType limitType)
        {
            _dataRecord = dataRecord;
            _columnLimit = columnLimit;
            _limitType = limitType;
        }

        public enum ColumnLimitType
        {
            FirstNColumns,
            AllButFirstNColumns
        }

        #region Implementation of IDataRecord

        public string GetName(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetName);
        }

        public string GetDataTypeName(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDataTypeName);
        }

        public Type GetFieldType(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetFieldType);
        }

        public object GetValue(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetValue);
        }

        public int GetValues(object[] values)
        {
            var actualCount = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < actualCount; i++)
            {
                values[i] = GetValue(i);
            }
            return actualCount;
        }

        public int GetOrdinal(string name)
        {
            var ordinalOfUnderlyingRecord = _dataRecord.GetOrdinal(name);

            switch (LimitType)
            {
                case ColumnLimitType.FirstNColumns:
                    if(ordinalOfUnderlyingRecord < _columnLimit)
                        return ordinalOfUnderlyingRecord;
                    break;
                case ColumnLimitType.AllButFirstNColumns:
                    if (ordinalOfUnderlyingRecord >= _columnLimit)
                        return ordinalOfUnderlyingRecord - _columnLimit;
                    break;
            }

            throw new IndexOutOfRangeException("Column found in underlying data record but was outside the set column index limit");
        }

        public bool GetBoolean(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetBoolean);
        }

        public byte GetByte(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetByte);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (i < 0 || i > FieldCount)
                throw new IndexOutOfRangeException();

            return _dataRecord.GetBytes(TranslateIndex(i), fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetChar);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (i < 0 || i > FieldCount)
                throw new IndexOutOfRangeException();

            return _dataRecord.GetChars(TranslateIndex(i), fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetGuid);
        }

        public short GetInt16(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt16);
        }

        public int GetInt32(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt32);
        }

        public long GetInt64(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt64);
        }

        public float GetFloat(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetFloat);
        }

        public double GetDouble(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDouble);
        }

        public string GetString(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetString);
        }

        public decimal GetDecimal(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDecimal);
        }

        public DateTime GetDateTime(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDateTime);
        }

        public IDataReader GetData(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetData);
        }

        public bool IsDBNull(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.IsDBNull);
        }

        public int FieldCount
        {
            get
            {
                switch (LimitType)
                {
                    case ColumnLimitType.FirstNColumns:
                        return Math.Min(_dataRecord.FieldCount, _columnLimit);
                    default:
                        return Math.Max(_dataRecord.FieldCount - _columnLimit, 0);
                }
            }
        }

        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        object IDataRecord.this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        #endregion

        internal ColumnLimitType LimitType
        {
            get { return _limitType; }
        }

        internal T CheckFieldCountAndCallBase<T>(int i, Func<int, T> functionToCall)
        {
            if (i < 0 || i > FieldCount)
                throw new IndexOutOfRangeException();

            return functionToCall(TranslateIndex(i));
        }

        private int TranslateIndex(int i)
        {
            switch (LimitType)
            {
                case ColumnLimitType.FirstNColumns:
                    return i;
                default:
                    return i + _columnLimit;
            }
        }
    }
}