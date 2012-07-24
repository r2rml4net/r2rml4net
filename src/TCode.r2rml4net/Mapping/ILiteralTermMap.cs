using System;

namespace TCode.r2rml4net.Mapping
{
    public interface ILiteralTermMap
    {
        Uri DataTypeURI { get; }
        string LanguageTag { get; }
    }
}