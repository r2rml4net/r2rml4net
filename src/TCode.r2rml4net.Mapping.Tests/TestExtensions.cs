using System;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.Tests
{
    static class TestExtensions
    {
        internal static Uri GetURI(this ITermTypeConfiguration config)
        {
            return ((ITermType) config).URI;
        }
    }
}
