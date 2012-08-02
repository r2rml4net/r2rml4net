using System;
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class SubjectMappingStrategy : MappingStrategyBase, ISubjectMappingStrategy
    {
        public SubjectMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of ISubjectMappingStrategy

        public virtual Uri CreateSubjectUri(Uri baseUri, TableMetadata table)
        {
            return CreateSubjectUri(baseUri, table.Name);
        }

        public virtual Uri CreateSubjectUri(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            return CreateSubjectUri(baseUri, foreignKey.TableName);
        }

        public virtual string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table)
        {
            var columnsArray = table.Select(c => c.Name).ToArray();

            if (!columnsArray.Any())
                throw new InvalidTriplesMapException(string.Format("No columns for table {0}", table.Name));

            var joinedColumnNames = string.Join(Options.TemplateSeparator, columnsArray.Select(DirectMappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", table.Name, Options.TemplateSeparator, joinedColumnNames);
        }

        public virtual string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table)
        {
            string template = DirectMappingHelper.UrlEncode(CreateSubjectUri(baseUri, table.Name).ToString());
            template += "/" + string.Join(";", table.PrimaryKey.Select(pk => pk.Name).Select(pk => string.Format("{0}={1}", DirectMappingHelper.UrlEncode(pk), DirectMappingHelper.EncloseColumnName(pk))));
            return template;
        }

        #endregion

        private Uri CreateSubjectUri(Uri baseUri, string tableName)
        {
            return new Uri(baseUri, DirectMappingHelper.UrlEncode(tableName));
        }
    }
}