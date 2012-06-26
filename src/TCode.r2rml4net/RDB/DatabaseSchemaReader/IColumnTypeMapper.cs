using DatabaseSchemaReader.DataSchema;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    /// <summary>
    /// Maps database column types to a member of <see cref="R2RMLType"/> enumeration
    /// </summary>
    public interface IColumnTypeMapper
    {
        /// <summary>
        /// Gets a literal type supported by R2RML equivalent to column's type
        /// </summary>
        /// <param name="dataType">Column <see cref="DataType"/></param>
        R2RMLType GetColumnTypeFromColumn(DataType dataType);
    }
}