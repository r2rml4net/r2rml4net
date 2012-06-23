using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation od <see cref="List{T}"/> and <see cref="IVistitable{T}"/> representing a group of tables
    /// </summary>
    public class TableCollection : IVistitable<IDatabaseMetadataVisitor>, IEnumerable<TableMetadata>
    {
        readonly List<TableMetadata> _tables = new List<TableMetadata>();

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
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public TableMetadata this[string tableName]
        {
            get
            {
                if (tableName == null)
                    throw new ArgumentNullException("tableName");
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentOutOfRangeException("tableName");

                var table = this.SingleOrDefault(t => t.Name == tableName);
                if (table == null)
                    throw new IndexOutOfRangeException(string.Format("TableCollection does not contain table {0}", tableName));

                return table;
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<TableMetadata> GetEnumerator()
        {
            return _tables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        internal void Add(TableMetadata table)
        {
            if (this.Any(tab => tab.Name == table.Name))
                throw new ArgumentException(string.Format("TableCollection already contains a table named {0}", table.Name));

            _tables.Add(table);
        }

        public int Count
        {
            get { return _tables.Count; }
        }
    }
}
