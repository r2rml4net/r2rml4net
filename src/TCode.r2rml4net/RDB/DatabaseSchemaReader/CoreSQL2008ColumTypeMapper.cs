using System;
using DatabaseSchemaReader.DataSchema;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    // todo: to be removed?
    public class CoreSQL2008ColumTypeMapper : IColumnTypeMapper
    {
        #region Implementation of IColumnTypeMapper

        public R2RMLType GetColumnTypeFromColumn(DataType dataType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}