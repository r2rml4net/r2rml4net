using System;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

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

        public virtual Uri CreateReferencePredicateUri(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            string uri = baseUri + DirectMappingHelper.UrlEncode(foreignKey.TableName) + "#ref-" + string.Join(".", foreignKey.ForeignKeyColumns.Select(DirectMappingHelper.UrlEncode));

            return new Uri(DirectMappingHelper.UrlEncode(uri));
        }

        public virtual string CreateReferenceObjectTemplate(Uri baseUri, ForeignKeyMetadata foreignKeys)
        {
            if (foreignKeys.ForeignKeyColumns.Length != foreignKeys.ReferencedColumns.Length)
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", foreignKeys), "foreignKeys");

            if (!foreignKeys.ForeignKeyColumns.Any())
                throw new ArgumentException("Empty foreign key", "foreignKeys");

            StringBuilder template = new StringBuilder(SubjectMappingStrategy.CreateSubjectUri(baseUri, foreignKeys) + "/");
            template.AppendFormat("{0}={1}", DirectMappingHelper.UrlEncode(foreignKeys.ReferencedColumns[0]), DirectMappingHelper.EncloseColumnName(foreignKeys.ForeignKeyColumns[0]));
            for (int i = 1; i < foreignKeys.ForeignKeyColumns.Count(); i++)
            {
                template.AppendFormat(";{0}={1}", DirectMappingHelper.UrlEncode(foreignKeys.ReferencedColumns[i]), DirectMappingHelper.EncloseColumnName(foreignKeys.ForeignKeyColumns[i]));
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