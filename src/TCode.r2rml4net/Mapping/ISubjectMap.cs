using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a subject map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-subject-map</remarks>
    public interface ISubjectMap : ITermMap, IUriValuedTermMap
    {
        /// <summary>
        /// Gets the graph maps associated with this <see cref="ISubjectMap"/>
        /// </summary>
        IEnumerable<IGraphMap> GraphMaps { get; }
        /// <summary>
        /// All classes added to this <see cref="ISubjectMap"/>
        /// </summary>
        Uri[] Classes { get; }
    }
}