using System;

namespace TCode.r2rml4net.RDF
{
    public interface IGraphMap : ITermMap
    {
        Uri Graph { get; }
    }
}