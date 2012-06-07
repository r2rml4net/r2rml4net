namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface IR2RMLConfiguration
    {
        ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename);
        ITriplesMapConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery);
    }
}