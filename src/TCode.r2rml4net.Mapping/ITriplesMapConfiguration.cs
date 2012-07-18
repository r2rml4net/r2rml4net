using System;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration of the Triples Map graph as described on http://www.w3.org/TR/r2rml/#triples-map
    /// </summary>
    public interface ITriplesMapConfiguration : ITriplesMap
    {
        /// <summary>
        /// Name of the table view which is source for triples as described on http://www.w3.org/TR/r2rml/#physical-tables
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// Query, which will be used as datasource as described on http://www.w3.org/TR/r2rml/#r2rml-views
        /// </summary>
        string SqlQuery { get; }
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

        IR2RMLConfiguration R2RMLConfiguration { get; }
    }
}