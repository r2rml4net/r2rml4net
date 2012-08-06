using System;

namespace TCode.r2rml4net.Mapping
{
    public interface ILiteralTermMap : ITermMap
    {
        Uri DataTypeURI { get; }
        string LanguageTag { get; }
        string Literal { get; }
    }
}