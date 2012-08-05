using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IPrimaryKeyMappingStrategy
    {
        Uri CreateSubjectUri(Uri baseUri, string tableName);
        string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table);
        string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table);
    }
}