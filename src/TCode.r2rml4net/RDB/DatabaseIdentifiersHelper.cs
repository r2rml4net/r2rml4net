using System.Text.RegularExpressions;

namespace TCode.r2rml4net.RDB
{
    static class DatabaseIdentifiersHelper
    {
        private static readonly char[] StartDelimiters = new[] { '`', '\"', '[' };
        private static readonly char[] EndDelimiters = new[] { '`', '\"', ']' };

        private static readonly Regex ColumnNameRegex = new Regex(@"^[\""`'\[](.+[^\""`'\]])[\""`'\]]$");

        internal static string GetColumnNameUnquoted(string columnName)
        {
            return columnName.TrimStart(StartDelimiters).TrimEnd(EndDelimiters);
        }

        internal static string DelimitIdentifier(string identifier, MappingOptions options)
        {
            if (options.UseDelimitedIdentifiers && !ColumnNameRegex.IsMatch(identifier))
                return string.Format("{0}{1}{2}", options.SqlIdentifierLeftDelimiter, identifier, options.SqlIdentifierRightDelimiter);

            return identifier; 
        }
    }
}
