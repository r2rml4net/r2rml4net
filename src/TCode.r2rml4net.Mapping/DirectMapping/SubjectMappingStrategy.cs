using System;
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
            if (table == null)
                throw new ArgumentNullException("table");

            return CreateSubjectUri(baseUri, table.Name);
        }

        public virtual Uri CreateSubjectUri(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");

            return CreateSubjectUri(baseUri, foreignKey.ReferencedTableName);
        }

        public virtual string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            var columnsArray = table.Select(c => c.Name).ToArray();

            if (!columnsArray.Any())
                throw new InvalidTriplesMapException(string.Format("No columns for table {0}", table.Name));

            var joinedColumnNames = string.Join(Options.TemplateSeparator, columnsArray.Select(DirectMappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", table.Name, Options.TemplateSeparator, joinedColumnNames);
        }

        public virtual string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (!table.PrimaryKey.Any())
                throw new ArgumentException(string.Format("Table {0} has no primary key", table.Name));

            string template = DirectMappingHelper.UrlEncode(CreateSubjectUri(baseUri, table.Name).ToString());
            template += "/" + string.Join(";", table.PrimaryKey.Select(pk => pk.Name).Select(pk => string.Format("{0}={1}", DirectMappingHelper.UrlEncode(pk), DirectMappingHelper.EncloseColumnName(pk))));
            return template;
        }

        #endregion

        private Uri CreateSubjectUri(Uri baseUri, string tableName)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (tableName == null)
                throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Invalid table name");

            return new Uri(baseUri, DirectMappingHelper.UrlEncode(tableName));
        }
    }
}