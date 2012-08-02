using System;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IColumnMappingStrategy
    {
        Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName);
    }
}