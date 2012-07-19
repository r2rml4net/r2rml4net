using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration of the Triples Map graph as described on http://www.w3.org/TR/r2rml/#triples-map
    /// </summary>
    public interface ITriplesMapConfiguration : ITriplesMap
    {
        /// <summary>
        /// <see cref="Uri"/> of the triples map represented by this instance
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// Adds a subject map subgraph to the mapping graph or returns existing if already created. Subject maps are used to construct subjects
        /// for triples procduced once mapping is applied to relational data
        /// <remarks>Triples map must contain exactly one subject map</remarks>
        /// </summary>
        new ISubjectMapConfiguration SubjectMap { get; }
        /// <summary>
        /// Adds a property-object map, which is used together with subject map to create complete triples\r\n
        /// from logical rows as described on http://www.w3.org/TR/r2rml/#predicate-object-map
        /// <remarks>Triples map can contain many property-object maps</remarks>
        /// </summary>
        IPredicateObjectMapConfiguration CreatePropertyObjectMap();
        /// <summary>
        /// The <see cref="IR2RMLConfiguration"/> containing this <see cref="ITriplesMapConfiguration"/>
        /// </summary>
        IR2RMLConfiguration R2RMLConfiguration { get; }
    }
}