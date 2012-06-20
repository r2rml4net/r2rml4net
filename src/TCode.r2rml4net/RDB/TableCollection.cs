using System;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation od <see cref="List{T}"/> and <see cref="IVistitable{T}"/> representing a group of tables
    /// </summary>
    public class TableCollection : List<TableMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        /// <summary>
        /// Visits self and all contained tables
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (TableMetadata table in this)
                table.Accept(visitor);
        }

        /// <summary>
        /// Gets the table with the specified name
        /// </summary>
        /// <exception cref="IndexOutOfRangeException" />
        public TableMetadata this[string tableName]
        {
            get
            {
                if (tableName == null)
                    throw new ArgumentNullException("tableName");
                if(string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentOutOfRangeException("tableName");

                var table = this.SingleOrDefault(t => t.Name == tableName);
                if (table == null)
                    throw new IndexOutOfRangeException(string.Format("TableCollection does not contain table {0}", tableName));

                return table;
            }
        }
    }
}
