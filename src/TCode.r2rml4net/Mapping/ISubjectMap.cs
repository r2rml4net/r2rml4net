using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping
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
        Uri URI { get; }
        /// <summary>
        /// Gets the graph maps associated with this <see cref="ISubjectMap"/>
        /// </summary>
        IEnumerable<IGraphMap> Graphs { get; }
    }
}