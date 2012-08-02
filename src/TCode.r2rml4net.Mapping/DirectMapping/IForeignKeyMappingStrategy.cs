using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IForeignKeyMappingStrategy
    {
        Uri CreateReferencePredicateUri(Uri baseUri, string tableName, IEnumerable<string> foreignKeyColumns);
        string CreateReferenceObjectTemplate(Uri mappingBaseUri, string referencedTableName, IEnumerable<string> foreignKeyColumns, IEnumerable<string> referencedColumns);
    }
}