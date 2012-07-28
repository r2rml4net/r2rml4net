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
    }
}
