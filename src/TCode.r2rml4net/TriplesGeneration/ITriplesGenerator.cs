namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for generating triples from R2RML mappings
    /// </summary>
    public interface ITriplesGenerator
    {
        /// <summary>
        /// Generates triples from <paramref name="r2RML"/> mappings
        /// </summary>
        VDS.RDF.ITripleStore GenerateTriples(Mapping.IR2RML r2RML);
    }
}