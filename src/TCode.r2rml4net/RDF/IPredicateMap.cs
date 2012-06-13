    using System;

namespace TCode.r2rml4net.RDF
{
    public interface IPredicateMap : ITermMap
    {
        Uri Predicate { get; }
    }
}