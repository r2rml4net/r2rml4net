using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Base for DotNetRDF-backed fluent R2RML configuration
    /// </summary>
    public abstract class BaseConfiguration
    {
        /// <summary>
        /// DotNetRDF graph containing the R2RML mappings
        /// </summary>
        protected internal IGraph R2RMLMappings { get; private set; }

        /// <summary>
        /// Constructor used by <see cref="R2RMLConfiguration"/>
        /// </summary>
        /// <param name="baseUri">R2RML graph's base URI</param>
        protected BaseConfiguration(Uri baseUri)
        {
            R2RMLMappings = new Graph { BaseUri = baseUri };
            EnsurePrefixes();
        }

        /// <summary>
        /// Constructor used by implementations other than <see cref="R2RMLConfiguration"/>
        /// </summary>
        protected BaseConfiguration(IGraph existingMappingsGraph)
        {
            R2RMLMappings = existingMappingsGraph;
            EnsurePrefixes();
        }

        private void EnsurePrefixes()
        {
            if (!R2RMLMappings.NamespaceMap.HasNamespace("rr"))
                R2RMLMappings.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
        }

        /// <summary>
        /// Reads all maps contained in the current configuration and creates configuration objects
        /// </summary>
        /// <remarks>Used in loading configuration from an exinsting graph</remarks>
        protected internal virtual void RecursiveInitializeSubMapsFromCurrentGraph()
        {
            InitializeSubMapsFromCurrentGraph();
        }

        protected abstract void InitializeSubMapsFromCurrentGraph();
    }
}
