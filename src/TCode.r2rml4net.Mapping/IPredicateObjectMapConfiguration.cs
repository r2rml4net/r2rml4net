using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration for predicate-object maps described on http://www.w3.org/TR/r2rml/#predicate-object-map
    /// </summary>
    public interface IPredicateObjectMapConfiguration
    {
        /// <summary>
        /// Creates a new object map
        /// </summary>
        /// <remarks><see cref="IPredicateObjectMapConfiguration"/> can have many object maps</remarks>
        IObjectMapConfiguration CreateObjectMap();
        /// <summary>
        /// Creates a new predicate map
        /// </summary>
        /// <remarks><see cref="IPredicateObjectMapConfiguration"/> can have many predicate maps</remarks>
        ITermMapConfiguration CreatePredicateMap();
        /// <summary>
        /// Creates an attached <see cref="IGraphMap"/>
        /// </summary>
        IGraphMap CreateGraphMap();
        /// <summary>
        /// Creates a new ref object map
        /// </summary>
        /// <remarks><see cref="IPredicateObjectMapConfiguration"/> can have many ref object maps</remarks>
        IRefObjectMapConfiguration CreateRefObjectMap(ITriplesMapConfiguration triplesMap);
    }
}