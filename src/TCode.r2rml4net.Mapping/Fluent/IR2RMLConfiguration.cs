namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Entrypoint to all of R2RML fluent configuration interfaces
    /// </summary>
    public interface IR2RMLConfiguration
    {
        ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename);
        ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery);
    }
}