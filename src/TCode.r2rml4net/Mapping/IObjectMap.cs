using System;
using TCode.r2rml4net.Exceptions;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents an object map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-object-map</remarks>
    public interface IObjectMap : ITermMap, ILiteralTermMap
    {
        /// <summary>
        /// Gets constant object URI or null if absent
        /// </summary>
        /// <exception cref="InvalidMapException"></exception>
        Uri URI { get; }
    }
}