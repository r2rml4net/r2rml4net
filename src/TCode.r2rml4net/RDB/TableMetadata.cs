using System;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a table from a relational database
    /// </summary>
    public class TableMetadata : IEnumerable<ColumnMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        private readonly IList<ColumnMetadata> _columns = new List<ColumnMetadata>();

        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Primary key column (or columns in case of composite key)
        /// </summary>
        /// todo: consider change type to string[]
        public ColumnMetadata[] PrimaryKey
        {
            get { return _columns.Where(c => c.IsPrimaryKey).ToArray(); }
        }

        /// <summary>
        /// Gets all the foreign keys
        /// </summary>
        public ForeignKeyMetadata[] ForeignKeys { get; internal set; }

        /// <summary>
        /// Visits self and contained columns
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (ColumnMetadata column in this)
                column.Accept(visitor);

            if (this.ForeignKeys != null)
                foreach (var foreignKey in ForeignKeys)
                {
                    visitor.Visit(foreignKey);
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
        /// Implemented to allow collection initialization
        /// </summary>
        internal void Add(ColumnMetadata column)
        {
            if (_columns.Any(col => col.Name == column.Name))
                throw new ArgumentException(string.Format("Table already contains column named {0}", column.Name));

            column.Table = this;
            _columns.Add(column);
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

                var column = this.SingleOrDefault(c => c.Name == columnName);
                if (column == null)
                    throw new IndexOutOfRangeException(string.Format("Table does not contain column {0}", columnName));

                return column;
            }
        }

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
    }
}
