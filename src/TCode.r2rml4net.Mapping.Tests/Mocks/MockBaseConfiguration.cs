using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mocks
{
    class MockBaseConfiguration : BaseConfiguration
    {
        public MockBaseConfiguration(IGraph graph, MappingOptions mappingOptions)
            : base(graph, mappingOptions)
        {
        }

        public MockBaseConfiguration(Uri baseUri, MappingOptions mappingOptions)
            : base(baseUri, mappingOptions)
        {
        }

        public MockBaseConfiguration(IGraph existingMappingsGraph, INode node, MappingOptions mappingOptions)
            : base(existingMappingsGraph, node, mappingOptions)
        {
        }

        public MockBaseConfiguration(ITriplesMapConfiguration triplesMap, IGraph existingMappingsGraph, INode node, MappingOptions mappingOptions)
            : base(triplesMap, existingMappingsGraph, node, mappingOptions)
        {
        }

        #region Overrides of BaseConfiguration

        /// <summary>
        /// Implemented in child classes should create submaps and for each of the run the <see cref="BaseConfiguration.RecursiveInitializeSubMapsFromCurrentGraph"/> method
        /// </summary>
        protected override void InitializeSubMapsFromCurrentGraph()
        {

        }

        #endregion
    }
}