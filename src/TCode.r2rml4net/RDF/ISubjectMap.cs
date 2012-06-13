using System;

namespace TCode.r2rml4net.RDF
{
    public interface ISubjectMap : ITermMap
    {
        Uri Subject { get; }
    }
}