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

        public ForeignKeyMetadata[] ForeignKeys { get; internal set; }

        /// <summary>
        /// Visits self and contained columns
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (ColumnMetadata column in this)
                column.Accept(visitor);

            if(this.ForeignKeys != null)
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
            column.Table = this;
            _columns.Add(column);
        }
    }
}
