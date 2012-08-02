using System.Web;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DirectMappingHelper
    {
        private DirectMappingOptions _options;

        public DirectMappingHelper(DirectMappingOptions options)
        {
            _options = options;
        }

        public string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString).Replace(" ", "%20");
        }

        public virtual string DelimitIdentifier(string identifier)
        {
            if (_options.UseDelimitedIdentifiers)
                return string.Format("{0}{1}{2}", _options.SqlIdentifierLeftDelimiter, identifier, _options.SqlIdentifierRightDelimiter);

            return identifier;
        }

        public virtual string EncloseColumnName(string columnName)
        {
            return string.Format("{{{0}}}", DelimitIdentifier(columnName));
        }
    }
}