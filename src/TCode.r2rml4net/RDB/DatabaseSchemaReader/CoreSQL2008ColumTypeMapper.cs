using System;
using DatabaseSchemaReader.DataSchema;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    // todo: to be removed?
    /// <summary>
    /// Implementation of <see cref="IColumnTypeMapper"/> which maps Core SQL:2008 datatypes
    /// </summary>
    public class CoreSQL2008ColumTypeMapper : IColumnTypeMapper
    {
        #region Implementation of IColumnTypeMapper

        /// <summary>
        /// Gets a member of <see cref="R2RMLType"/> enumeration for a given Core SQL:2008 <see cref="DataType"/>
        /// </summary>
        /// <remarks>Expects db typename to be one of the values described on http://www.w3.org/TR/r2rml/#natural-mapping</remarks>
        public R2RMLType GetColumnTypeFromColumn(DataType dataType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}