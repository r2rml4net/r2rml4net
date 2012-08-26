using System;

namespace TCode.r2rml4net
{
    public sealed class MappingOptions
    {
        private string _templateSeparator;
        private const string DefaultTemplateSeparator = "_";
        private const char DefaultIdentifierDelimiter = '\"';

        public MappingOptions()
        {
            TemplateSeparator = DefaultTemplateSeparator;
            SqlIdentifierRightDelimiter = DefaultIdentifierDelimiter;
            SqlIdentifierLeftDelimiter = DefaultIdentifierDelimiter;
            UseDelimitedIdentifiers = true;
            ValidateSqlVersion = true;
            IgnoreMappingErrors = true;
            IgnoreDataErrors = true;
            PreserveDuplicateRows = false;
        }

        public string TemplateSeparator
        {
            get { return _templateSeparator; }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                _templateSeparator = value;
            }
        }

        public bool UseDelimitedIdentifiers { get; set; }

        public char SqlIdentifierRightDelimiter { get; private set; }

        public char SqlIdentifierLeftDelimiter { get; private set; }

        public bool ValidateSqlVersion { get; set; }

        public bool IgnoreMappingErrors { get; set; }

        public bool IgnoreDataErrors { get; set; }

        public bool PreserveDuplicateRows { get; set; }

        public void SetSqlIdentifierDelimiters(char newLeftDelimiter, char newRightDelimiter)
        {
            SqlIdentifierLeftDelimiter = newLeftDelimiter;
            SqlIdentifierRightDelimiter = newRightDelimiter;
        }
    }
}