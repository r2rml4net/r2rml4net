using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface ISubjectMappingStrategy
    {
        Uri CreateSubjectUri(Uri baseUri, TableMetadata table);
        string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table);
        string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table);
        Uri CreateSubjectUri(Uri baseUri, ForeignKeyMetadata foreignKey);
    }
}