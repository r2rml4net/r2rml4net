using System;
using System.Collections.Generic;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a triples map
    /// </summary>
    /// <remarks>See more at http://www.w3.org/TR/r2rml/#dfn-triples-map</remarks>
    public interface ITriplesMap : IMapBase, IQueryMap
    {
        /// <summary>
        /// Gets predicate-object maps associated with this <see cref="ITriplesMap"/>
        /// </summary>
        IEnumerable<IPredicateObjectMap> PredicateObjectMaps { get; }

        /// <summary>
        /// Gets the subject map associated with this <see cref="ITriplesMap"/>
        /// </summary>
        ISubjectMap SubjectMap { get; }

        /// <summary>
        /// Name of the table view which is source for triples
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#physical-tables</remarks>
        string TableName { get; }

        /// <summary>
        /// Query, which will be used as source for triples
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        string SqlQuery { get; }

        /// <summary>
        /// Gets the URIs of SQL versions set for the logical table
        /// </summary>
        Uri[] SqlVersions { get; }
    }
}