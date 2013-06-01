using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace TCode.r2rml4net.Configuration
{
    internal class MappingOptionsLoader
    {
        private static readonly IDictionary<Uri, Action<MappingOptions, IValuedNode>> PropertySetters;

        static MappingOptionsLoader()
        {
            PropertySetters = new Dictionary<Uri, Action<MappingOptions, IValuedNode>>
            {
                {Ontology.BlankNodeTemplateSeparator, SetBNodeSeparator},
                {Ontology.UseDelimitedIdentifiers, SetUseDelimitedIdentifiers},
                {Ontology.SqlIdentifierDelimiter, SetSqlIdentifierDelimiter},
                {Ontology.ValidateSqlVersion, SetValidateSqlVersion},
                {Ontology.IgnoreMappingErrors, SetIgnoreMappingErrors},
                {Ontology.IgnoreDataErrors, SetIgnoreDataErrors},
                {Ontology.PreserveDuplicateRows, SetPreserveDuplicateRows},
            };
        }

        public MappingOptions Load(IGraph configGraph, INode mappingOptionsNode)
        {
            var mappingOptions = new MappingOptions();

            foreach (var triple in configGraph.GetTriplesWithSubject(mappingOptionsNode))
            {
                var predicate = (IUriNode)triple.Predicate;
                var objectNode = triple.Object.AsValuedNode();

                if (PropertySetters.ContainsKey(predicate.Uri))
                {
                    PropertySetters[predicate.Uri](mappingOptions, objectNode);
                }
            }

            return mappingOptions;
        }

        private static void SetPreserveDuplicateRows(MappingOptions options, IValuedNode node)
        {
            options.PreserveDuplicateRows = node.AsBoolean();
        }

        private static void SetIgnoreDataErrors(MappingOptions options, IValuedNode node)
        {
            options.IgnoreDataErrors = node.AsBoolean();
        }

        private static void SetIgnoreMappingErrors(MappingOptions options, IValuedNode node)
        {
            options.IgnoreMappingErrors = node.AsBoolean();
        }

        private static void SetValidateSqlVersion(MappingOptions options, IValuedNode node)
        {
            options.ValidateSqlVersion = node.AsBoolean();
        }

        private static void SetSqlIdentifierDelimiter(MappingOptions options, IValuedNode node)
        {
            var delimiter = node.AsString().First();
            options.SetSqlIdentifierDelimiters(delimiter, delimiter);
        }

        private static void SetUseDelimitedIdentifiers(MappingOptions options, IValuedNode node)
        {
            options.UseDelimitedIdentifiers = node.AsBoolean();
        }

        private static void SetBNodeSeparator(MappingOptions options, IValuedNode node)
        {
            options.BlankNodeTemplateSeparator = node.AsString();
        }
    }
}