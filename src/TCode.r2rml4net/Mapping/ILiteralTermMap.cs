using System;

namespace TCode.r2rml4net.Mapping
{
    public interface ILiteralTermMap : ITermMap
    {
        Uri DataTypeURI { get; }
        string Language { get; }
        string Literal { get; }
    }
}