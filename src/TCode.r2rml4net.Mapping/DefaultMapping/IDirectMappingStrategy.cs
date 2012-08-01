using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping.DefaultMapping
{
    public interface IDirectMappingStrategy
    {
        Uri CreateUriForTable(Uri baseUri, string tableName);
        string CreateTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns);
    }
}