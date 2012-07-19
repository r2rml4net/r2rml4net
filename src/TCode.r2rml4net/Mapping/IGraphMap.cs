using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a graph map 
    /// </summary>
    /// <remarks>See more at http://www.w3.org/TR/r2rml/#dfn-graph-map</remarks>
    public interface IGraphMap : ITermMap
    {
        /// <summary>
        /// Get the GraphUri URI or null if no URI has been set
        /// </summary>
        Uri URI { get; }
    }
}