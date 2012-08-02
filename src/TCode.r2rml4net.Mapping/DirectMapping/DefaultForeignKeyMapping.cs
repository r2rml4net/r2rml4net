using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultForeignKeyMapping : MappingStrategyBase, IForeignKeyMappingStrategy
    {
        private ISubjectMappingStrategy _subjectMappingStrategy;

        public DefaultForeignKeyMapping(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IForeignKeyMappingStrategy

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

        #endregion

        public ISubjectMappingStrategy SubjectMappingStrategy
        {
            get
            {
                if (_subjectMappingStrategy == null)
                    _subjectMappingStrategy = new DefaultSubjectMapping(Options);

                return _subjectMappingStrategy;
            }
            set { _subjectMappingStrategy = value; }
        }
    }
}