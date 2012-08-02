using System;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultSubjectMappingStrategy : DirectMappingStrategyBase, ISubjectMappingStrategy
    {
        public DefaultSubjectMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of ISubjectMappingStrategy

        public virtual Uri CreateSubjectUri(Uri baseUri, string tableName)
        {
            return new Uri(baseUri, DirectMappingHelper.UrlEncode(tableName));
        }

        public virtual string CreateSubjectTemplateForNoPrimaryKey(string tableName, IEnumerable<string> columns)
        {
            var columnsArray = columns as string[] ?? columns.ToArray();

            if (!columnsArray.Any())
                throw new InvalidTriplesMapException(string.Format("No columns for table {0}", tableName));

            var joinedColumnNames = string.Join(Options.TemplateSeparator, columnsArray.Select(DirectMappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", tableName, Options.TemplateSeparator, joinedColumnNames);
        }

        public virtual string CreateSubjectTemplateForPrimaryKey(Uri baseUri, string tableName, IEnumerable<string> primaryKeyColumns)
        {
            string template = DirectMappingHelper.UrlEncode(CreateSubjectUri(baseUri, tableName).ToString());
            template += "/" + string.Join(";", primaryKeyColumns.Select(pk => string.Format("{0}={1}", DirectMappingHelper.UrlEncode(pk), DirectMappingHelper.EncloseColumnName(pk))));
            return template;
        }

        #endregion
    }
}