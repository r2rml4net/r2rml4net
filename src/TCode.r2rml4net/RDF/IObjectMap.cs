using System;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents an object map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-object-map</remarks>
    public interface IObjectMap : ITermMap
    {
        /// <summary>
        /// Gets constant object URI or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        Uri Object { get; }
        /// <summary>
        /// Gets constant object literal value or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string Literal { get; }
    }
}