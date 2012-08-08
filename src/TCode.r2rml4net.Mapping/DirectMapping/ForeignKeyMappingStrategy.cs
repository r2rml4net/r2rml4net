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
                throw new ArgumentException("Invalid referencing table's name");
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

            if (foreignKey.IsCandidateKeyReference && (!foreignKey.ReferencedTableHasPrimaryKey || !foreignKey.ReferencedTable.PrimaryKey.Any()))
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference between tables {0} and {1} but table {1} has no primary key",
                        foreignKey.TableName, foreignKey.ReferencedTable.Name));

            string[] referencedColumns = foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.Select(c => c.Name).ToArray()
                : foreignKey.ReferencedColumns;
            string[] foreignKeyColumns = foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.Select(c => string.Format("{0}{1}", foreignKey.ReferencedTable.Name, c.Name)).ToArray()
                : foreignKey.ForeignKeyColumns;

            StringBuilder template = new StringBuilder(PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, foreignKey.ReferencedTable.Name) + "/");
            template.AppendFormat("{0}={1}", MappingHelper.UrlEncode(referencedColumns[0]), MappingHelper.EncloseColumnName(foreignKeyColumns[0]));
            for (int i = 1; i < foreignKeyColumns.Length; i++)
            {
                template.AppendFormat(";{0}={1}", MappingHelper.UrlEncode(referencedColumns[i]), MappingHelper.EncloseColumnName(foreignKeyColumns[i]));
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