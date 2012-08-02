using System;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultColumnMapping : IColumnMappingStrategy
    {
        #region Implementation of IColumnMappingStrategy

        public Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName)
        {
            string predicateUriString = string.Format("{0}{1}#{2}", baseUri, tableName, columnName);
            return new Uri(predicateUriString);
        }

        #endregion
    }
}