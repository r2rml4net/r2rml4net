using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Interface for classes implementating the algorithm of mapping primary keys to RDF subjects
    /// </summary>
    public interface IPrimaryKeyMappingStrategy
    {
        /// <summary>
        /// Create an absolute URI subject for a table
        /// </summary>
        Uri CreateSubjectClassUri(Uri baseUri, string tableName);
        /// <summary>
        /// Creates a blank node identifier template for a table without primary key
        /// </summary>
        string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table);
        /// <summary>
        /// Creates a URI template for table with primary key
        /// </summary>
        string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table);
    }
}