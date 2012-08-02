using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultColumnMapping : IColumnMappingStrategy
    {
        #region Implementation of IColumnMappingStrategy

        public virtual Uri CreatePredicateUri(Uri baseUri, ColumnMetadata column)
        {
            string predicateUriString = string.Format("{0}{1}#{2}", baseUri, column.Table.Name, column.Name);
            return new Uri(predicateUriString);
        }

        #endregion
    }
}