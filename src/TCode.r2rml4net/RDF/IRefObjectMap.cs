using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a RefObjectMap
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-referencing-object-map</remarks>
    public interface IRefObjectMap : IMapBase
    {
        /// <summary>
        /// Optional join conditions associated with this <see cref="IRefObjectMap"/>
        /// </summary>
        IEnumerable<JoinCondition> JoinConditions { get; }
    }
}
