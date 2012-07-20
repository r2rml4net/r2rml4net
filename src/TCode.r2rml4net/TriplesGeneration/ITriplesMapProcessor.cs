using System.Collections.Generic;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface ITriplesMapProcessor
    {
        IEnumerable<IGraph> ProcessTriplesMap(ITriplesMap triplesMap);
    }
}