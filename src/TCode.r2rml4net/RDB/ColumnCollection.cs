using System;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    public class ColumnCollection : IEnumerable<ColumnMetadata>
    {
        private readonly IList<ColumnMetadata> _columns = new List<ColumnMetadata>();

        /// <summary>
        /// Gets the table's columns count
        /// </summary>
        public int ColumnsCount
        {
            get
            {
                return _columns.Count;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/>
        /// </summary>
        public IEnumerator<ColumnMetadata> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/>
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        /// <summary>
        /// Gets the column with the specified name
        /// </summary>
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public ColumnMetadata this[string columnName]
        {
            get
            {
                if (columnName == null)
                    throw new ArgumentNullException("columnName");
                if (string.IsNullOrWhiteSpace(columnName))
                    throw new ArgumentOutOfRangeException("columnName");

                var column = Enumerable.SingleOrDefault<ColumnMetadata>(this, c => c.Name == columnName);
                if (column == null)
                    throw new IndexOutOfRangeException(string.Format("Table does not contain column {0}", columnName));

                return column;
            }
        }

        /// <summary>
        /// Implemented to allow collection initialization
        /// </summary>
        protected internal virtual void Add(ColumnMetadata column)
        {
            if (_columns.Any(col => col.Name == column.Name))
                throw new ArgumentException(string.Format("Collection already contains column {0}", column.Name));

            _columns.Add(column);
        }
    }
}