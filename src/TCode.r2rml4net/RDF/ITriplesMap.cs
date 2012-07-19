using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a triples map
    /// </summary>
    /// <remarks>See more at http://www.w3.org/TR/r2rml/#dfn-triples-map</remarks>
    public interface ITriplesMap : IMapBase
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
        /// Gets the effective sql query based od <see cref="TableName"/> or <see cref="SqlQuery"/>
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        string EffectiveSqlQuery { get; }

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
    }
}