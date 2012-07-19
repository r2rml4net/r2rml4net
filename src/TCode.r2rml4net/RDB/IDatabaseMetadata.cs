namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Interface for reading and accessing structure of a relational database such as MSSQL, MySQL, Oracle etc.
    /// </summary>
    public interface IDatabaseMetadata
    {
        /// <summary>
        /// Collecion of tables contained by the current database
        /// </summary>
        TableCollection Tables { get; }
    }
}
