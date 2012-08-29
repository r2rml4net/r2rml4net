#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

using System;
using System.Data;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// A wrapper for <see cref="IDataRecord"/>, which makes it possible to access it as if it contained
    /// only first N columns or all but first N columns
    /// </summary>
    /// <remarks>This is implemented as part of the algorithm described in <a href="http://www.w3.org/TR/r2rml/#generated-triples">11.1 section of R2RML specification</a></remarks>
    public class ColumnConstrainedDataRecord : IDataRecord
    {
        private readonly IDataRecord _dataRecord;
        private readonly int _columnLimit;
        private readonly ColumnLimitType _limitType;

        /// <summary>
        /// Creates a new instance of <see cref="ColumnConstrainedDataRecord"/> wrapper
        /// </summary>
        /// <param name="dataRecord">wrapped data row</param>
        /// <param name="columnLimit"></param>
        /// <param name="limitType">first N columns or all but first N column</param>
        public ColumnConstrainedDataRecord(IDataRecord dataRecord, int columnLimit, ColumnLimitType limitType)
        {
            _dataRecord = dataRecord;
            _columnLimit = columnLimit;
            _limitType = limitType;
        }

        /// <summary>
        /// Type of constraining the <see cref="IDataRecord"/>
        /// </summary>
        public enum ColumnLimitType
        {
            /// <summary>
            /// Filter <see cref="IDataRecord"/> to only return first N columns
            /// </summary>
            FirstNColumns,
            /// <summary>
            /// Filter <see cref="IDataRecord"/> to return all but first N columns
            /// </summary>
            AllButFirstNColumns
        }

        #region Implementation of IDataRecord

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetName(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetName);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDataTypeName);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public Type GetFieldType(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetFieldType);
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public object GetValue(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetValue);
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        /// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into. </param><filterpriority>2</filterpriority>
        public int GetValues(object[] values)
        {
            var actualCount = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < actualCount; i++)
            {
                values[i] = GetValue(i);
            }
            return actualCount;
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        /// <param name="name">The name of the field to find. </param><filterpriority>2</filterpriority>
        public int GetOrdinal(string name)
        {
            switch (LimitType)
            {
                case ColumnLimitType.FirstNColumns:
                    var ordinalOfUnderlyingRecord = _dataRecord.GetOrdinal(name);
                    if (ordinalOfUnderlyingRecord < _columnLimit)
                        return ordinalOfUnderlyingRecord;
                    break;
                case ColumnLimitType.AllButFirstNColumns:
                    for (int i = _columnLimit; i < _dataRecord.FieldCount; i++)
                    {
                        if (_dataRecord.GetName(i) == name)
                            return i - _columnLimit;
                    }
                    break;
            }

            throw new IndexOutOfRangeException("Column found in underlying data record but was outside the set column index limit");
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <returns>
        /// The value of the column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public bool GetBoolean(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetBoolean);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public byte GetByte(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetByte);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of bytes read.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><param name="fieldOffset">The index within the field from which to start the read operation. </param><param name="buffer">The buffer into which to read the stream of bytes. </param><param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation. </param><param name="length">The number of bytes to read. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (i < 0 || i > FieldCount)
                throw new IndexOutOfRangeException();

            return _dataRecord.GetBytes(TranslateIndex(i), fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public char GetChar(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetChar);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of characters read.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><param name="fieldoffset">The index within the row from which to start the read operation. </param><param name="buffer">The buffer into which to read the stream of bytes. </param><param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation. </param><param name="length">The number of bytes to read. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (i < 0 || i > FieldCount)
                throw new IndexOutOfRangeException();

            return _dataRecord.GetChars(TranslateIndex(i), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public Guid GetGuid(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetGuid);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public short GetInt16(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt16);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public int GetInt32(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt32);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetInt64(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetInt64);
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public float GetFloat(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetFloat);
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public double GetDouble(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDouble);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetString(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetString);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public decimal GetDecimal(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDecimal);
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public DateTime GetDateTime(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetDateTime);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public IDataReader GetData(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.GetData);
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public bool IsDBNull(int i)
        {
            return CheckFieldCountAndCallBase(i, _dataRecord.IsDBNull);
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <returns>
        /// When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
        /// </returns>
        /// <filterpriority>2</filterpriority>
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

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <returns>
        /// The column located at the specified index as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="i">The zero-based index of the column to get. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <returns>
        /// The column with the specified name as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="name">The name of the column to find. </param><exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception><filterpriority>2</filterpriority>
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