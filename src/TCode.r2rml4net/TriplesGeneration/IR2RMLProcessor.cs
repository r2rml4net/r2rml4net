using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for generating triples from R2RML mappings
    /// </summary>
    public interface IR2RMLProcessor
    {
        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and processes them with the given <see cref="IRdfHandler"/>
        /// </summary>
        void GenerateTriples(IR2RML r2RML, IRdfHandler rdfHandler);
        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and returns the generated dataset
        /// </summary>
        ITripleStore GenerateTriples(IR2RML r2RML);
        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings and adds the generated triples to the given <see cref="ITripleStore"/>
        /// </summary>
        void GenerateTriples(IR2RML r2RML, ITripleStore tripleStore);
        /// <summary>
        /// Gets a value indicating whether generating triples had no errors
        /// </summary>
        bool Success { get; }
    }
}