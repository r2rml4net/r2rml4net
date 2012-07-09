using System;
using System.IO;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    public static class R2RMLLoader
    {
        public static IR2RML Load(string r2RMLGraph)
        {
            IGraph graph = new Graph();
            graph.LoadFromString(r2RMLGraph);

            return InitializeMappings(graph);
        }

        public static IR2RML Load(Stream r2RMLGraph)
        {
            using(var reader  =new StreamReader(r2RMLGraph))
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
