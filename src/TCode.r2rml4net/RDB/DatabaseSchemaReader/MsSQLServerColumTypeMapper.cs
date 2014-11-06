#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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
using System.Linq;
using DatabaseSchemaReader.DataSchema;
using NullGuard;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    /// <summary>
    /// Implementation of <see cref="IColumnTypeMapper"/> which maps Microsoft SQL Server datatypes
    /// </summary>
    public class MSSQLServerColumTypeMapper : IColumnTypeMapper
    {
        #region Implementation of IColumnTypeMapper

        /// <summary>
        /// Gets a member of <see cref="R2RMLType"/> enumeration for a given MS SQL Server <see cref="DataType"/>
        /// </summary>
        public R2RMLType GetColumnTypeFromColumn([AllowNull] DataType dataType)
        {
            if (dataType != null)
            {
                if (dataType.IsString || dataType.IsStringClob)
                    return R2RMLType.String;

                Type type = dataType.GetNetType();

                if (new[] { typeof(int), typeof(short), typeof(long), typeof(sbyte) }.Contains(type))
                    return R2RMLType.Integer;

                if (dataType.IsDateTime || dataType.GetNetType() == typeof(DateTimeOffset))
                {
                    if (dataType.TypeName.Equals("date", StringComparison.OrdinalIgnoreCase))
                        return R2RMLType.Date;

                    return R2RMLType.DateTime;
                }

                if (new[] { typeof(float), typeof(double) }.Contains(type))
                    return R2RMLType.FloatingPoint;

                if (type == typeof(decimal))
                    return R2RMLType.Decimal;

                if (type == typeof(TimeSpan))
                    return R2RMLType.Time;

                if (dataType.GetNetType() == typeof(byte[]))
                    return R2RMLType.Binary;

                if (dataType.GetNetType() == typeof(bool))
                    return R2RMLType.Boolean;
            }

            return R2RMLType.Undefined;
        }

        #endregion
    }
}