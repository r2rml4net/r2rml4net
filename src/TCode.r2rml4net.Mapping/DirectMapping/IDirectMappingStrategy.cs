using System;
using System.Collections.Generic;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IDirectMappingStrategy
    {
        Uri CreateSubjectUri(Uri baseUri, string tableName);
        string CreateSubjectTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns);
        Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName);
        string CreateSubjectTemplateForPrimaryKey(Uri mappingBaseUri, string tableName, IEnumerable<string> primaryKeyColumns);
        Uri CreateReferencePredicateUri(Uri baseUri, string tableName, IEnumerable<string> foreignKeyColumns);
        string CreateReferenceObjectTemplate(Uri mappingBaseUri, string referencedTableName, IEnumerable<string> foreignKeyColumns, IEnumerable<string> referencedColumns);
        void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table);
        void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table);
    }
}