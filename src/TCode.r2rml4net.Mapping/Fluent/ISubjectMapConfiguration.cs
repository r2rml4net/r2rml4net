using System;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Interface for creating configuration of subject maps as described on http://www.w3.org/TR/r2rml/#subject-map
    /// </summary>
    public interface ISubjectMapConfiguration : ITermMapConfiguration
    {
        /// <summary>
        /// Adds a class definition to subject map. A subject map can have many classes which will be used to construct
        /// triples for each RDF term as described on http://www.w3.org/TR/r2rml/#typing
        /// </summary>
        ISubjectMapConfiguration AddClass(Uri classIri);
        /// <summary>
        /// All classes added to this subject map
        /// </summary>
        Uri[] ClassIris { get; }
    }
}