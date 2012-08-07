using System.Web;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net
{
    public class MappingHelper
    {
        private readonly MappingOptions _options;

        public MappingHelper(MappingOptions options)
        {
            _options = options;
        }

        public string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString).Replace(" ", "%20");
        }

        public virtual string EncloseColumnName(string columnName)
        {
            return string.Format("{{{0}}}", DatabaseIdentifiersHelper.DelimitIdentifier(columnName, _options));
        }
    }
}