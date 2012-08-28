using System;

namespace TCode.r2rml4net
{
    // todo: elaborate in comments below

    /// <summary>
    /// Represents a set of options for <a href="http://www.w3.org/TR/r2rml/#default-mappings">default mapping generation</a> 
    /// and <a href="http://www.w3.org/TR/r2rml/#generated-rdf">triples generation</a>
    /// </summary>
    public sealed class MappingOptions
    {
        private string _blankNodeTemplateSeparator;
        private const string DefaultTemplateSeparator = "_";
        private const char DefaultIdentifierDelimiter = '\"';

        /// <summary>
        /// Creates a new instance of <see cref="MappingOptions"/> with default options' values
        /// </summary>
        public MappingOptions()
        {
            BlankNodeTemplateSeparator = DefaultTemplateSeparator;
            SqlIdentifierRightDelimiter = DefaultIdentifierDelimiter;
            SqlIdentifierLeftDelimiter = DefaultIdentifierDelimiter;
            UseDelimitedIdentifiers = true;
            ValidateSqlVersion = true;
            IgnoreMappingErrors = true;
            IgnoreDataErrors = true;
            PreserveDuplicateRows = false;
        }

        /// <summary>
        /// Gets or sets the string which will be used for building blank node <a href="http://www.w3.org/TR/r2rml/#from-template">templates</a>.
        /// Default value is "_"
        /// </summary>
        public string BlankNodeTemplateSeparator
        {
            get { return _blankNodeTemplateSeparator; }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                _blankNodeTemplateSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether delimited SQL identifiers should be used in mapping graphs
        /// </summary>
        public bool UseDelimitedIdentifiers { get; set; }

        /// <summary>
        /// Gets the right SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierRightDelimiter { get; private set; }

        /// <summary>
        /// Gets the left SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierLeftDelimiter { get; private set; }

        /// <summary>
        /// Gets or sets value indicating whether <a href="http://www.w3.org/TR/r2rml/#dfn-sql-version-identifier">SQL version identifier</a> should be validated.
        /// Default value is true
        /// </summary>
        public bool ValidateSqlVersion { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether mapping errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreMappingErrors { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether data errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreDataErrors { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether duplicate rows in tables without primary key <a href="http://www.w3.org/TR/r2rml/#default-mappings">should be preserved</a>.
        /// Default value is false
        /// </summary>
        public bool PreserveDuplicateRows { get; set; }

        /// <summary>
        /// Sets the SQL identifier delimiters
        /// </summary>
        public void SetSqlIdentifierDelimiters(char newLeftDelimiter, char newRightDelimiter)
        {
            SqlIdentifierLeftDelimiter = newLeftDelimiter;
            SqlIdentifierRightDelimiter = newRightDelimiter;
        }
    }
}