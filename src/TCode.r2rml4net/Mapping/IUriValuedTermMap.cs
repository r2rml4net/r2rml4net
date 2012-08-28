using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>, which can be URI valued
    /// </summary>
    public interface IUriValuedTermMap : ITermMap
    {
        /// <summary>
        /// Get the URI
        /// </summary>
        Uri URI { get; }
    }
}