using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCode.r2rml4net.Mapping.DefaultMapping
{
    class DefaultMappingStrategy : IDirectMappingStrategy
    {
        #region Implementation of IDirectMappingStrategy

        public Uri CreateSubjectUri(Uri baseUri, string tableName)
        {
            return new Uri(baseUri, UrlEncode(tableName));
        }

        public string CreateSubjectTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns)
        {
            var joinedColumnNames = string.Join("_", columns);
            return string.Format("{0}_{1}", tableName, joinedColumnNames);
        }

        public Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName)
        {
            string predicateUriString = string.Format("{0}{1}#{2}", baseUri, tableName, columnName);
            return new Uri(predicateUriString);
        }

        public string CreateSubjectTemplateForPrimaryKey(Uri baseUri, string tableName, IEnumerable<string> primaryKeyColumns)
        {
            string template = UrlEncode(CreateSubjectUri(baseUri, tableName).ToString());
            template += "/" + string.Join(";", primaryKeyColumns.Select(pk => string.Format("{0}={{{1}}}", UrlEncode(pk), pk)));
            return template;
        }

        #endregion

        protected string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString).Replace(" ", "%20");
        }
    }
}