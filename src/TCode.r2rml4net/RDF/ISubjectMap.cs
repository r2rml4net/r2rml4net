using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a subject map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-subject-map</remarks>
    public interface ISubjectMap : ITermMap
    {
        /// <summary>
        /// Gets the subject URI or null if not set
        /// </summary>
        Uri Subject { get; }

        IEnumerable<IGraphMap> Graphs { get; }
    }
}