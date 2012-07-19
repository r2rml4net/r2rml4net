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
            IGraph graph = new Graph();
            graph.LoadFromString(r2RMLGraph);

            return InitializeMappings(graph);
        }

        /// <summary>
        /// Loads R2RML mappings from a stream
        /// </summary>
        public static IR2RML Load(Stream r2RMLGraph)
        {
            using (var reader = new StreamReader(r2RMLGraph))
            {
                return Load(reader.ReadToEnd());
            }
        }

        private static IR2RML InitializeMappings(IGraph graph)
        {
            var mappings = new R2RMLConfiguration(graph);
            mappings.RecursiveInitializeSubMapsFromCurrentGraph(null);
            return mappings;
        }
    }
}
