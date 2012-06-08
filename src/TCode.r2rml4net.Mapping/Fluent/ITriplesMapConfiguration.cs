namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Configuration of the Triples Map graph as described on http://www.w3.org/TR/r2rml/#triples-map
    /// </summary>
    public interface ITriplesMapConfiguration
    {
        /// <summary>
        /// Name of the table view which is source for triples as described on http://www.w3.org/TR/r2rml/#physical-tables
        /// </summary>
        string TableName { get; }
        string SqlQuery { get; }
    }
}