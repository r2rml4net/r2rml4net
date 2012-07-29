using System;
using System.Data;

namespace TCode.r2rml4net.RDB.ADO.NET
{
    public class UnquotedColumnDataRecordWrapper : IDataRecord
    {
        private readonly IDataRecord _wrapped;

        public UnquotedColumnDataRecordWrapper(IDataRecord wrapped)
        {
            _wrapped = wrapped;
        }

        #region Implementation of IDataRecord

        public string GetName(int i)
        {
            return _wrapped.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _wrapped.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _wrapped.GetFieldType(i);
        }

        public int GetValues(object[] values)
        {
            return _wrapped.GetValues(values);
        }

        public object GetValue(int i)
        {
            return _wrapped.GetValue(i);
        }

        public int GetOrdinal(string name)
        {
            return _wrapped.GetOrdinal(EnsureColumnNameUnquoted(name));
        }

        public bool GetBoolean(int i)
        {
            return _wrapped.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _wrapped.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _wrapped.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _wrapped.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _wrapped.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _wrapped.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _wrapped.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _wrapped.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _wrapped.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _wrapped.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _wrapped.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _wrapped.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _wrapped.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _wrapped.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _wrapped.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _wrapped.IsDBNull(i);
        }

        public int FieldCount
        {
            get { return _wrapped.FieldCount; }
        }

        public object this[int i]
        {
            get { return _wrapped[i]; }
        }

        public object this[string name]
        {
            get { return _wrapped[EnsureColumnNameUnquoted(name)]; }
        }

        #endregion

        private string EnsureColumnNameUnquoted(string columnName)
        {
            return DatabaseIdentifiersHelper.GetColumnNameUnquoted(columnName);
        }

    }
}