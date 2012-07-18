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
    }
}