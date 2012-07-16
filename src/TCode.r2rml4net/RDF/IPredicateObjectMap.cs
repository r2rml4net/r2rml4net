using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a predicate-object map
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-predicate-object-map</remarks>
    public interface IPredicateObjectMap
    {
        /// <summary>
        /// Gets object maps associated with this <see cref="IPredicateObjectMap"/>
        /// </summary>
        IEnumerable<IObjectMap> ObjectMaps { get; }
        /// <summary>
        /// Gets ref object maps associated with this <see cref="IPredicateObjectMap"/>
        /// </summary>
        IEnumerable<IRefObjectMap> RefObjectMaps { get; }
        /// <summary>
        /// Gets predicate maps associated with this <see cref="IPredicateObjectMap"/>
        /// </summary>
        IEnumerable<IPredicateMap> PredicateMaps { get; }
    }
}