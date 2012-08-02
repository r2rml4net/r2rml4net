using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
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
            var joinedColumnNames = string.Join("_", columns.Select(col => string.Format("{{{0}}}", col)));
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

        public Uri CreateReferencePredicateUri(Uri baseUri, string tableName, IEnumerable<string> foreignKeyColumns)
        {
            string uri = baseUri + UrlEncode(tableName) + "#ref-" + string.Join(".", foreignKeyColumns.Select(UrlEncode));

            return new Uri(UrlEncode(uri));
        }

        public string CreateReferenceObjectTemplate(Uri baseUri, string tableName, IEnumerable<string> foreignKey, IEnumerable<string> referencedPrimaryKey)
        {
            foreignKey = foreignKey.ToArray();
            referencedPrimaryKey = referencedPrimaryKey.ToArray();

            if (foreignKey.Count() != referencedPrimaryKey.Count())
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", tableName), "foreignKey");

            if (!foreignKey.Any())
                throw new ArgumentException("Empty foreign key", "foreignKey");

            StringBuilder template = new StringBuilder(CreateSubjectUri(baseUri, tableName) + "/");
            template.AppendFormat("{0}={{{1}}}", UrlEncode(referencedPrimaryKey.ElementAt(0)), foreignKey.ElementAt(0));
            for (int i = 1; i < foreignKey.Count(); i++)
            {
                template.AppendFormat(";{0}={{{1}}}", UrlEncode(referencedPrimaryKey.ElementAt(1)), foreignKey.ElementAt(1));
            }

            return UrlEncode(template.ToString());
        }

        public void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            string template = CreateSubjectTemplateForNoPrimaryKey(
                    table.Name,
                    table.Select(col => col.Name));
            var classIri = CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        public void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = CreateSubjectUri(baseUri, table.Name);

            string template = CreateSubjectTemplateForPrimaryKey(
                baseUri,
                table.Name,
                table.PrimaryKey.Select(pk => pk.Name));

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        #endregion

        protected string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString).Replace(" ", "%20");
        }
    }
}