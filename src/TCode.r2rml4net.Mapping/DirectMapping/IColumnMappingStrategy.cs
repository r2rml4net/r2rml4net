using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Interface for classes implementating the algorithm of columns to predicates
    /// </summary>
    public interface IColumnMappingStrategy
    {
        /// <summary>
        /// Creates a predicate URI for a column
        /// </summary>
        Uri CreatePredicateUri(Uri baseUri, ColumnMetadata column);
    }
}