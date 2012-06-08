using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    abstract class BaseConfiguration
    {
        protected internal IGraph R2RMLMappings { get; private set; }

        protected BaseConfiguration(Uri baseUri)
        {
            R2RMLMappings = new Graph { BaseUri = baseUri };
            R2RMLMappings.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
        }

        protected BaseConfiguration(IGraph existingMappingsGraph)
        {
            R2RMLMappings = existingMappingsGraph;
        }
    }
}
