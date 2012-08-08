using System;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class ForeignKeyMappingStrategy : MappingStrategyBase, IForeignKeyMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;

        public ForeignKeyMappingStrategy(MappingOptions options)
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

            string uri = baseUri + MappingHelper.UrlEncode(foreignKey.TableName) + "#ref-" + string.Join(";", foreignKey.ForeignKeyColumns.Select(MappingHelper.UrlEncode));

            return new Uri(MappingHelper.UrlEncode(uri));
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
                        foreignKey.TableName, foreignKey.ReferencedTable.Name));

            StringBuilder template = new StringBuilder(PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, foreignKey.ReferencedTable.Name) + "/");
            template.AppendFormat("{0}={1}", MappingHelper.UrlEncode(foreignKey.ReferencedColumns[0]), MappingHelper.EncloseColumnName(foreignKey.ForeignKeyColumns[0]));
            for (int i = 1; i < foreignKey.ForeignKeyColumns.Count(); i++)
            {
                template.AppendFormat(";{0}={1}", MappingHelper.UrlEncode(foreignKey.ReferencedColumns[i]), MappingHelper.EncloseColumnName(foreignKey.ForeignKeyColumns[i]));
            }

            return MappingHelper.UrlEncode(template.ToString());
        }

        public string CreateObjectTemplateForCandidateKeyReference(ForeignKeyMetadata foreignKey)
        {
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");
            if (!foreignKey.IsCandidateKeyReference)
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference expected but was primary key reference between tables {0} and {1}",
                        foreignKey.TableName, foreignKey.ReferencedTable.Name));
            
            return CreateBlankNodeTemplate(foreignKey.ReferencedTable.Name, foreignKey.ReferencedColumns);
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