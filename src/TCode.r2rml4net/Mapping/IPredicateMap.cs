using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a predicate map
    /// </summary>
    /// <remarks>See more on http://www.w3.org/TR/r2rml/#dfn-predicate-map</remarks>
    public interface IPredicateMap : ITermMap
    {
        /// <summary>
        /// Gets the predicate's URI or null if not set
        /// </summary>
        Uri URI { get; }
    }
}