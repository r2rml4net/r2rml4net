using System;

namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface ITriplesMapFromR2RMLViewConfiguration : ITriplesMapConfiguration
    {
        ITriplesMapConfiguration SetSqlVersion(Uri uri);
        ITriplesMapConfiguration SetSqlVersion(string uri);
    }
}