using System.Collections.Generic;
using VDS.RDF;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a triples map
    /// </summary>
    /// <remarks>See more at http://www.w3.org/TR/r2rml/#dfn-triples-map</remarks>
    public interface ITriplesMap
    {
        IEnumerable<IPredicateObjectMap> PredicateObjectMaps { get; }
        ISubjectMap SubjectMap { get; }
        INode Node { get; }
    }
}