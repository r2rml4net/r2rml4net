using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a triples map
    /// </summary>
    /// <remarks>See more at http://www.w3.org/TR/r2rml/#dfn-triples-map</remarks>
    public interface ITriplesMap : IMapBase
    {
        IEnumerable<IPredicateObjectMap> PredicateObjectMaps { get; }
        ISubjectMap SubjectMap { get; }
        string EffectiveSqlQuery { get; }

        /// <summary>
        /// Name of the table view which is source for triples as described on http://www.w3.org/TR/r2rml/#physical-tables
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Query, which will be used as datasource as described on http://www.w3.org/TR/r2rml/#r2rml-views
        /// </summary>
        string SqlQuery { get; }
    }
}