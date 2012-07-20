using System.Collections.Generic;
using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface ITriplesMapProcessor
    {
        void ProcessTriplesMap(ITriplesMap triplesMap, IDbConnection connection);
    }
}