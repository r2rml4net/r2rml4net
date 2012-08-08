namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a foreign key
    /// </summary>
    public class ForeignKeyMetadata
    {
        /// <summary>
        /// Gets the table containing this foreign key
        /// </summary>
        public string TableName { get; internal set; }
        /// <summary>
        /// Gets the foreign key column
        /// </summary>
        public string[] ForeignKeyColumns { get; internal set; }
        /// <summary>
        /// Get the referenced table
        /// </summary>
        public TableMetadata ReferencedTable { get; internal set; }
        /// <summary>
        /// Gets the referenced columns
        /// </summary>
        public string[] ReferencedColumns { get; internal set; }
        /// <summary>
        /// Returns true if the referenced columns do not form a primary key
        /// </summary>
        public bool IsCandidateKeyReference { get; internal set; }

        public bool ReferencedTableHasPrimaryKey { get; internal set; }
    }
}