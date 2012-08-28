using System;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Default implementation of <see cref="IForeignKeyMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class ForeignKeyMappingStrategy : MappingStrategyBase, IForeignKeyMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;

        /// <summary>
        /// Creates an instance of <see cref="ForeignKeyMappingStrategy"/>
        /// </summary>
        public ForeignKeyMappingStrategy(MappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IForeignKeyMappingStrategy

        /// <summary>
        /// Creates a predicate URI for foreign Key according to <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
        /// </summary>
        /// <example>For referenced table "Student", foreign key columns "Last Name" and "SSN" and base URI "http://www.exmample.com/" it creates a 
        /// URI "http://www.exmample.com/Student#ref-{\"Last Name\"};{\"SSN\"}"</example>
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

            return new Uri(uri);
        }

        /// <summary>
        /// Creates an object template for a foreign key reference
        /// </summary>
        /// <remarks>The template contains both referenced and referencing columns. Different columns are used
        /// if the referenced table has or hasn't got a primary key producing different templates</remarks>
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

            string[] referencedColumns = foreignKey.IsCandidateKeyReference && foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.ToArray()
                : foreignKey.ReferencedColumns;
            string[] foreignKeyColumns = foreignKey.IsCandidateKeyReference && foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.Select(c => string.Format("{0}{1}", foreignKey.ReferencedTable.Name, c)).ToArray()
                : foreignKey.ForeignKeyColumns;

            StringBuilder template = new StringBuilder(PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, foreignKey.ReferencedTable.Name) + "/");
            template.AppendFormat("{0}={1}", MappingHelper.UrlEncode(referencedColumns[0]), MappingHelper.EncloseColumnName(foreignKeyColumns[0]));
            for (int i = 1; i < foreignKeyColumns.Length; i++)
            {
                template.AppendFormat(";{0}={1}", MappingHelper.UrlEncode(referencedColumns[i]), MappingHelper.EncloseColumnName(foreignKeyColumns[i]));
            }

            return template.ToString();
        }

        /// <summary>
        /// Creates a blank node object identifier template for foreign key, which references a candidate key.
        /// See <see cref="MappingStrategyBase.CreateBlankNodeTemplate"/> for details on implementation
        /// </summary>
        public string CreateObjectTemplateForCandidateKeyReference(ForeignKeyMetadata foreignKey)
        {
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");
            if (!foreignKey.IsCandidateKeyReference)
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference expected but was primary key reference between tables {0} and {1}",
                        foreignKey.TableName, foreignKey.ReferencedTable.Name));

            return CreateBlankNodeTemplate(foreignKey.ReferencedTable.Name, foreignKey.ForeignKeyColumns);
        }

        #endregion

        /// <summary>
        /// Mapping strategy for primary keys
        /// </summary>
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