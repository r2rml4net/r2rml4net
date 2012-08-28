namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Class representing a unique key
    /// </summary>
    public class UniqueKeyMetadata : ColumnCollection
    {
        /// <summary>
        /// Gets value indicating whether the unique key is referenced by a foreign key
        /// </summary>
        public bool IsReferenced { get; internal set; }
    }
}