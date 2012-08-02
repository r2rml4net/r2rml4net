using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface ISubjectMappingStrategy
    {
        Uri CreateSubjectUri(Uri baseUri, string tableName);
        string CreateSubjectTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns);
        string CreateSubjectTemplateForPrimaryKey(Uri mappingBaseUri, string tableName, IEnumerable<string> primaryKeyColumns);
    }
}