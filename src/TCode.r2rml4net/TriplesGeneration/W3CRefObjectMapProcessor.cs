using System.Collections.Generic;
using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Default implementation of <see cref="IRefObjectMapProcessor"/> generating triples for joined triples maps
    /// </summary>
    /// <remarks>see http://www.w3.org/TR/r2rml/#generated-triples</remarks>
    class W3CRefObjectMapProcessor : MapProcessorBase, IRefObjectMapProcessor
    {
        public W3CRefObjectMapProcessor(IRdfHandler rdfHandler)
            : base(rdfHandler)
        {
        }

        #region Implementation of IRefObjectMapProcessor

        public void ProcessRefObjectMap(IRefObjectMap refObjectMap, IDbConnection dbConnection, IEnumerable<IGraphMap> predicateObjectMapGraphMaps)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}