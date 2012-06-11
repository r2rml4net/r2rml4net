using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    /// <summary>
    /// Base for DotNetRDF-backed fluent R2RML configuration
    /// </summary>
    public abstract class BaseConfiguration
    {
        internal const string RdfType = "rdf:type";
        internal const string RrIRI = "rr:IRI";
        internal const string RrTermTypeProperty = "rr:termType";
        internal const string RrLogicalTableProperty = "rr:logicalTable";
        internal const string RrTableNameProperty = "rr:tableName";
        internal const string RrSqlQueryProperty = "rr:sqlQuery";
        internal const string RrSqlVersionProperty = "rr:sqlVersion";
        internal const string RrTriplesMapClass = "rr:TriplesMap";
        internal const string RrClassProperty = "rr:class";
        internal const string RrSubjectMapProperty = "rr:subjectMap";
        internal const string RrBlankNode = "rr:BlankNode";
        internal const string RrLiteral = "rr:Literal";
        internal const string RrSubjectProperty = "rr:subject";
        internal const string RrPredicatePropety = "rr:predicate";
        internal const string RrObjectProperty = "rr:object";
        internal const string RrGraphPropety = "rr:graph";
        internal const string RrPredicateMapPropety = "rr:predicateMap";
        internal const string RrObjectMapProperty = "rr:objectMap";
        internal const string RrGraphMapPropety = "rr:graphMap";
        internal const string RrColumnProperty = "rr:column";
        internal const string RrPredicateObjectMapPropety = "rr:predicateObjectMap";
        internal const string RrLanguageTagPropety = "rr:languageTag";
        internal const string RrDatatypePropety = "rr:datatype";

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
    }
}
