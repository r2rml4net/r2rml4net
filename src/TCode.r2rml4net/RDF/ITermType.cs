using System;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a term type
    /// </summary>
    /// <remarks>See more on http://www.w3.org/TR/r2rml/#dfn-term-type</remarks>
    public interface ITermType
    {
        /// <summary>
        /// Returns term type set with configuration
        /// or a default value
        /// </summary>
        /// <remarks>Default value is described on http://www.w3.org/TR/r2rml/#dfn-term-type</remarks>
        Uri URI { get; }
    }
}