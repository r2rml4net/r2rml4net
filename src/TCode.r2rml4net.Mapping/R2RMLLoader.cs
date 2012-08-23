using System.IO;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Class for loading existing R2RML mappings
    /// </summary>
    public static class R2RMLLoader
    {
        /// <summary>
        /// Loads R2RML mappings from a string
        /// </summary>
        public static IR2RML Load(string r2RMLGraph)
        {
            return Load(r2RMLGraph, new MappingOptions());
        }

        /// <summary>
        /// Loads R2RML mappings from a stream
        /// </summary>
        public static IR2RML Load(Stream r2RMLGraph)
        {
            return Load(r2RMLGraph, new MappingOptions());
        }
        /// <summary>
        /// Loads R2RML mappings from a string
        /// </summary>
        public static IR2RML Load(string r2RMLGraph, MappingOptions mappingOptions)
        {
            IGraph graph = new Graph();
            graph.LoadFromString(r2RMLGraph);

            return InitializeMappings(graph, mappingOptions);
        }

        /// <summary>
        /// Loads R2RML mappings from a stream
        /// </summary>
        public static IR2RML Load(Stream r2RMLGraph, MappingOptions mappingOptions)
        {
            using (var reader = new StreamReader(r2RMLGraph))
            {
                return Load(reader.ReadToEnd(), mappingOptions);
            }
        }

        private static IR2RML InitializeMappings(IGraph graph, MappingOptions mappingOptions)
        {
            var mappings = new R2RMLConfiguration(graph, mappingOptions);
            mappings.RecursiveInitializeSubMapsFromCurrentGraph();
            return mappings;
        }
    }
}
