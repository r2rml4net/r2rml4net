using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Default implementation of <see cref="IColumnMappingStrategy"/>,  which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class ColumnMappingStrategy : IColumnMappingStrategy
    {
        #region Implementation of IColumnMappingStrategy

        /// <summary>
        /// Creates a predicate URI for a <paramref name="column"/>
        /// </summary>
        /// <example>For table 'Student', column 'Last name' and base URI 'http://example.com/' it returns URI 'http://example.com/Student#Last%20name'</example>
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