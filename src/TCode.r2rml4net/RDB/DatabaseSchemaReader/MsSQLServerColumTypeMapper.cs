using System;
using System.Linq;
using DatabaseSchemaReader.DataSchema;

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
        public R2RMLType GetColumnTypeFromColumn(DataType dataType)
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