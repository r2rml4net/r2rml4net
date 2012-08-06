using System;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public sealed class DirectMappingOptions
    {
        private string _templateSeparator;
        private const string DefaultTemplateSeparator = "_";
        private const char DefaultIdentifierDelimiter = '\"';

        internal DirectMappingOptions()
        {
            TemplateSeparator = DefaultTemplateSeparator;
            SqlIdentifierRightDelimiter = DefaultIdentifierDelimiter;
            SqlIdentifierLeftDelimiter = DefaultIdentifierDelimiter;
            UseDelimitedIdentifiers = true;
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

        public void SetSqlIdentifierDelimiters(char newLeftDelimiter, char newRightDelimiter)
        {
            SqlIdentifierLeftDelimiter = newLeftDelimiter;
            SqlIdentifierRightDelimiter = newRightDelimiter;
        }
    }
}