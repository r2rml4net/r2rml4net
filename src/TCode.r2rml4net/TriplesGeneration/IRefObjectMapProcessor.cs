using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface IRefObjectMapProcessor
    {
        void ProcessRefObjectMap(IRefObjectMap refObjectMap, ISubjectMap subjectMap, IDbConnection dbConnection, int childColumnsCount, IRdfHandler rdfHandler);
    }
}