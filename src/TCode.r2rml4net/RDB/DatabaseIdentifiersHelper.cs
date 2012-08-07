using System.Text.RegularExpressions;

namespace TCode.r2rml4net.RDB
{
    static class DatabaseIdentifiersHelper
    {
        private static readonly Regex ColumnNameRegex = new Regex(@"^[\""`'\[]+([_ a-zA-Z0-9]+)[\""`'\]]+$");

        internal static string GetColumnNameUnquoted(string columnName)
        {
            return ColumnNameRegex.Replace(columnName, "$1");
        }

        internal static string DelimitIdentifier(string identifier, MappingOptions options)
        {
            if (options.UseDelimitedIdentifiers && !ColumnNameRegex.IsMatch(identifier))
                return string.Format("{0}{1}{2}", options.SqlIdentifierLeftDelimiter, identifier, options.SqlIdentifierRightDelimiter);

            return identifier; 
        }
    }
}
