using System.Collections.Generic;
using System.Data;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface IRefObjectMapProcessor
    {
        void ProcessRefObjectMap(IRefObjectMap refObjectMap, IDbConnection dbConnection, IEnumerable<IGraphMap> predicateObjectMapGraphMaps);
    }
}