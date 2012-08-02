using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultMappingStrategy : DirectMappingStrategyBase, IDirectMappingStrategy, IColumnMappingStrategy, IForeignKeyMappingStrategy
    {
        public DefaultMappingStrategy()
            : this(new DirectMappingOptions())
        {
        }

        public DefaultMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
            SubjectMappingStrategy = new DefaultSubjectMappingStrategy(options);
        }

        #region Implementation of IDirectMappingStrategy

        public virtual Uri CreatePredicateUri(Uri baseUri, string tableName, string columnName)
        {
            string predicateUriString = string.Format("{0}{1}#{2}", baseUri, tableName, columnName);
            return new Uri(predicateUriString);
        }

        public virtual Uri CreateReferencePredicateUri(Uri baseUri, string tableName, IEnumerable<string> foreignKeyColumns)
        {
            string uri = baseUri + DirectMappingHelper.UrlEncode(tableName) + "#ref-" + string.Join(".", foreignKeyColumns.Select(DirectMappingHelper.UrlEncode));

            return new Uri(DirectMappingHelper.UrlEncode(uri));
        }

        public virtual string CreateReferenceObjectTemplate(Uri baseUri, string tableName, IEnumerable<string> foreignKey, IEnumerable<string> referencedPrimaryKey)
        {
            foreignKey = foreignKey.ToArray();
            referencedPrimaryKey = referencedPrimaryKey.ToArray();

            if (foreignKey.Count() != referencedPrimaryKey.Count())
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", tableName), "foreignKey");

            if (!foreignKey.Any())
                throw new ArgumentException("Empty foreign key", "foreignKey");

            StringBuilder template = new StringBuilder(SubjectMappingStrategy.CreateSubjectUri(baseUri, tableName) + "/");
            template.AppendFormat("{0}={1}", DirectMappingHelper.UrlEncode(referencedPrimaryKey.ElementAt(0)), DirectMappingHelper.EncloseColumnName(foreignKey.ElementAt(0)));
            for (int i = 1; i < foreignKey.Count(); i++)
            {
                template.AppendFormat(";{0}={1}", DirectMappingHelper.UrlEncode(referencedPrimaryKey.ElementAt(i)), DirectMappingHelper.EncloseColumnName(foreignKey.ElementAt(i)));
            }

            return DirectMappingHelper.UrlEncode(template.ToString());
        }

        public virtual void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            string template = SubjectMappingStrategy.CreateSubjectTemplateForNoPrimaryKey(
                    table.Name,
                    table.Select(col => col.Name));
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        public virtual void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            string template = SubjectMappingStrategy.CreateSubjectTemplateForPrimaryKey(
                baseUri,
                table.Name,
                table.PrimaryKey.Select(pk => pk.Name));

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        #endregion

        public ISubjectMappingStrategy SubjectMappingStrategy { get; set; }
    }
}