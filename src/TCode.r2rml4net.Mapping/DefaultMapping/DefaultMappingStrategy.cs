using System;
using System.Collections.Generic;
using System.Web;

namespace TCode.r2rml4net.Mapping.DefaultMapping
{
    class DefaultMappingStrategy : IDirectMappingStrategy
    {
        #region Implementation of IDirectMappingStrategy

        public Uri CreateUriForTable(Uri baseUri, string tableName)
        {
            return new Uri(baseUri, UrlEncode(tableName));
        }

        public string CreateTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns)
        {
            var joinedColumnNames = string.Join("_", columns);
            return string.Format("{0}_{1}", tableName, joinedColumnNames);
        }

        #endregion

        protected string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString).Replace(" ", "%20");
        }
    }
}