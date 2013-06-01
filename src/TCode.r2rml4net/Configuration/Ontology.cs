using System;
using VDS.RDF;

namespace TCode.r2rml4net.Configuration
{
    public static class Ontology
    {
        public static readonly Uri ConnectionString = UriFactory.Create("http://r2rml.net/configuration#connectionString");
        public static readonly Uri ConnectionType = UriFactory.Create("http://r2rml.net/configuration#connectionType");
        public static readonly Uri MappingOptions = UriFactory.Create("http://r2rml.net/configuration#mappingOptions");
        public static readonly Uri BlankNodeTemplateSeparator = UriFactory.Create("http://r2rml.net/configuration#blankNodeTemplateSeparator");
        public static readonly Uri UseDelimitedIdentifiers = UriFactory.Create("http://r2rml.net/configuration#useDelimitedIdentifiers");
        public static readonly Uri SqlIdentifierDelimiter = UriFactory.Create("http://r2rml.net/configuration#sqlIdentifierDelimiter");
        public static readonly Uri ValidateSqlVersion = UriFactory.Create("http://r2rml.net/configuration#validateSqlVersion");
        public static readonly Uri IgnoreMappingErrors = UriFactory.Create("http://r2rml.net/configuration#ignoreMappingErrors");
        public static readonly Uri IgnoreDataErrors = UriFactory.Create("http://r2rml.net/configuration#ignoreDataErrors");
        public static readonly Uri PreserveDuplicateRows = UriFactory.Create("http://r2rml.net/configuration#preserveDuplicateRows");
    }
}