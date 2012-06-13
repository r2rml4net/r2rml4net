using System;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Provides read-only access to term maps
    /// </summary>
    public interface ITermMap
    {
        /// <summary>
        /// Gets template or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string Template { get; }

        ITermType TermType { get; }
    }
}