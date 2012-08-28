using System;
using System.Linq;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping.Log;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Default implementation of <see cref="IPrimaryKeyMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class PrimaryKeyMappingStrategy : MappingStrategyBase, IPrimaryKeyMappingStrategy
    {
        /// <summary>
        /// Default mapping log
        /// </summary>
        public IDefaultMappingGenerationLog Log { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="PrimaryKeyMappingStrategy"/>
        /// </summary>
        public PrimaryKeyMappingStrategy(MappingOptions options)
            : base(options)
        {
            Log = NullLog.Instance;
        }

        #region Implementation of IPrimaryKeyMappingStrategy

        /// <summary>
        /// Creates a URI for subject class by joining <paramref name="baseUri"/> and <paramref name="tableName"/>
        /// </summary>
        public Uri CreateSubjectClassUri(Uri baseUri, string tableName)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (tableName == null)
                throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Invalid table name");

            return new Uri(baseUri, MappingHelper.UrlEncode(tableName));
        }

        /// <summary>
        /// Creates a blank node identifier subject template by concatenating the referenced table name with the referenced columns
        /// </summary>
        /// <example>For table "Student" and referenced columns "Last Name" and "SSN" it creates a template "Student;{\"Last Name\"};{\"SSN\"}"</example>
        /// <remarks>If the referenced table has multiple unique keys the template will be created for the longest one. <br/>
        /// If the referenced table has no unique key, all columns are used</remarks>
        public virtual string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            var uniqueKeys = table.UniqueKeys.ToArray();
            var referencedUniqueKeys = uniqueKeys.Where(uq => uq.IsReferenced).ToArray();
            if (referencedUniqueKeys.Length > 1)
                Log.LogMultipleCompositeKeyReferences(table);

            ColumnCollection columnsForTemplate;

            if (uniqueKeys.Any())
            {
                if (referencedUniqueKeys.Length == 1)
                    columnsForTemplate = referencedUniqueKeys.Single();
                else
                    columnsForTemplate = uniqueKeys.OrderBy(c => c.ColumnsCount).First();
            }
            else
            {
                columnsForTemplate = table;
            }

            var columnsArray=columnsForTemplate.Select(c => c.Name).ToArray();
            var name = table.Name;
            if (!columnsArray.Any())
                throw new InvalidMapException(string.Format("No columns for table {0}", name));

            return CreateBlankNodeTemplate(name, columnsArray);
        }

        /// <summary>
        /// Creates a blank node identifier subject template for referenced table with primary key
        /// </summary>
        public virtual string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (!table.PrimaryKey.Any())
                throw new ArgumentException(string.Format("Table {0} has no primary key", table.Name));

            string template = CreateSubjectClassUri(baseUri, table.Name).OriginalString;
            template += "/" + string.Join(";", table.PrimaryKey.Select(pk => string.Format("{0}={1}", MappingHelper.UrlEncode(pk), MappingHelper.EncloseColumnName(pk))));
            return template;
        }

        #endregion
    }
}