using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for classes which process <see cref="IRefObjectMap"/>s in order to generate RDF triples
    /// </summary>
    public interface IRefObjectMapProcessor
    {
        /// <summary>
        /// Retrieves source data using the given <paramref name="dbConnection"/> and processes <paramref name="refObjectMap"/>
        /// to transform that data to RDF triples
        /// </summary>
        void ProcessRefObjectMap(IRefObjectMap refObjectMap, ISubjectMap subjectMap, IDbConnection dbConnection, int childColumnsCount, IRdfHandler rdfHandler);
    }
}