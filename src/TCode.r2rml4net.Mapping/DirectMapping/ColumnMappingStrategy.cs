using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class ColumnMappingStrategy : IColumnMappingStrategy
    {
        #region Implementation of IColumnMappingStrategy

        public virtual Uri CreatePredicateUri(Uri baseUri, ColumnMetadata column)
        {
            if(baseUri == null)
                throw new ArgumentNullException("baseUri");
            if(column == null)
                throw new ArgumentNullException("column");
            if(string.IsNullOrWhiteSpace(column.Name))
                throw new ArgumentException("Column name invalid", "column");

            string predicateUriString = string.Format("{0}#{1}", column.Table.Name, column.Name);
            return new Uri(baseUri, predicateUriString);
        }

        #endregion
    }
}