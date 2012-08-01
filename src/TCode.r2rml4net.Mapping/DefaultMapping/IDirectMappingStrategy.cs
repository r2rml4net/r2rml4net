using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping.DefaultMapping
{
    public interface IDirectMappingStrategy
    {
        Uri CreateSubjectUri(Uri baseUri, string tableName);
        string CreateSubjectTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns);
        Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName);
    }
}