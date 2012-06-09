using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Base for DotNetRDF-backed fluent R2RML configuration
    /// </summary>
    public abstract class BaseConfiguration
    {
        protected const string RdfType = "rdf:type";
        protected const string RrLogicalTableProperty = "rr:logicalTable";
        protected const string RrTableNameProperty = "rr:tableName";
        protected const string RrSqlQueryProperty = "rr:sqlQuery";
        protected const string RrSqlVersionProperty = "rr:sqlVersion";
        protected const string RrTriplesMapClass = "rr:TriplesMap";
        protected const string RrClassClass = "rr:Class";
        protected const string RrSubjectMapProperty = "rr:subjectMap";

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
    }
}
