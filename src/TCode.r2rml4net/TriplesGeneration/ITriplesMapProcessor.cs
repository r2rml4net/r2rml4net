using System.Data;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for classes which process <see cref="ITriplesMap"/> in order to generate RDF triples
    /// </summary>
    public interface ITriplesMapProcessor
    {
        /// <summary>
        /// Retrieves source data using the given <paramref name="connection"/> and process <paramref name="triplesMap"/>
        /// to transform that data to RDF triples
        /// </summary>
        void ProcessTriplesMap(ITriplesMap triplesMap, IDbConnection connection);
    }
}