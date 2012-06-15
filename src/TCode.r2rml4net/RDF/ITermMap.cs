using System;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a term map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-term-map</remarks>
    public interface ITermMap
    {
        /// <summary>
        /// Gets template or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string Template { get; }
        /// <summary>
        /// Gets the term map's <see cref="ITermType"/>
        /// </summary>
        ITermType TermType { get; }
        /// <summary>
        /// Gets column or null if not set
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string ColumnName { get; }
    }
}