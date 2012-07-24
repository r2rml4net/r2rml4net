using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping
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

        /// <summary>
        /// Returns effective sql query of the child triples map
        /// </summary>
        /// <remarks>http://www.w3.org/TR/r2rml/#foreign-key</remarks>
        string ChildEffectiveSqlQuery { get; }

        /// <summary>
        /// Returns effective sql query of the parent triples map
        /// </summary>
        /// <remarks>http://www.w3.org/TR/r2rml/#foreign-key</remarks>
        string ParentEffectiveSqlQuery { get; }

        /// <summary>
        string EffectiveSqlQuery { get; }
        /// Returns the referenced triples map's subject
        /// </summary>
        ISubjectMap SubjectMap { get; }

        IPredicateObjectMap PredicateObjectMap { get; }
    }
}
