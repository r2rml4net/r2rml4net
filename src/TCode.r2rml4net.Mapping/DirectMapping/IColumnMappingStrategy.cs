using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IColumnMappingStrategy
    {
        Uri CreatePredicateUri(Uri baseUri, ColumnMetadata column);
    }
}