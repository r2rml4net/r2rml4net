using System;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a table from a relational database
    /// </summary>
    public class TableMetadata : ColumnCollection, IVistitable<IDatabaseMetadataVisitor>
    {
        public TableMetadata()
        {
            ForeignKeys = new ForeignKeyMetadata[0];
            UniqueKeys = new UniqueKeyCollection();
        }

        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Primary key column (or columns in case of composite key)
        /// </summary>
        public string[] PrimaryKey
        {
            get { return this.Where(c => c.IsPrimaryKey).Select(c => c.Name).ToArray(); }
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

        protected internal override void Add(ColumnMetadata column)
        {
            base.Add(column);
            column.Table = this;
        }

        public UniqueKeyCollection UniqueKeys { get; internal set; }
    }
}
