using System;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class ForeignKeyMappingStrategy : MappingStrategyBase, IForeignKeyMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;

        public ForeignKeyMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IForeignKeyMappingStrategy

        public virtual Uri CreateReferencePredicateUri(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");
            if (string.IsNullOrWhiteSpace(foreignKey.TableName))
                throw new ArgumentException("Invalid referenced table's name");
            if (!foreignKey.ForeignKeyColumns.Any())
                throw new ArgumentException("Empty foreign key", "foreignKey");

            string uri = baseUri + DirectMappingHelper.UrlEncode(foreignKey.TableName) + "#ref-" + string.Join(";", foreignKey.ForeignKeyColumns.Select(DirectMappingHelper.UrlEncode));

            return new Uri(DirectMappingHelper.UrlEncode(uri));
        }

        public virtual string CreateReferenceObjectTemplate(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            if (!foreignKey.ForeignKeyColumns.Any())
                throw new ArgumentException("Empty foreign key", "foreignKey");

            if (foreignKey.ForeignKeyColumns.Length != foreignKey.ReferencedColumns.Length)
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", foreignKey.TableName), "foreignKey");

            if (foreignKey.IsCandidateKeyReference)
                throw new ArgumentException(
                    string.Format(
                        "Primary key reference expected but was canditate key reference between tables {0} and {1}",
                        foreignKey.TableName, foreignKey.ReferencedTableName));

            StringBuilder template = new StringBuilder(PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, foreignKey.ReferencedTableName) + "/");
            template.AppendFormat("{0}={1}", DirectMappingHelper.UrlEncode(foreignKey.ReferencedColumns[0]), DirectMappingHelper.EncloseColumnName(foreignKey.ForeignKeyColumns[0]));
            for (int i = 1; i < foreignKey.ForeignKeyColumns.Count(); i++)
            {
                template.AppendFormat(";{0}={1}", DirectMappingHelper.UrlEncode(foreignKey.ReferencedColumns[i]), DirectMappingHelper.EncloseColumnName(foreignKey.ForeignKeyColumns[i]));
            }

            return DirectMappingHelper.UrlEncode(template.ToString());
        }

        public string CreateObjectTemplateForCandidateKeyReference(ForeignKeyMetadata foreignKey)
        {
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");
            if (!foreignKey.IsCandidateKeyReference)
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference expected but was primary key reference between tables {0} and {1}",
                        foreignKey.TableName, foreignKey.ReferencedTableName));
            
            return CreateBlankNodeTemplate(foreignKey.ReferencedTableName, foreignKey.ReferencedColumns);
        }

        #endregion

        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy(Options);

                return _primaryKeyMappingStrategy;
            }
            set { _primaryKeyMappingStrategy = value; }
        }
    }
}