namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Entrypoint to all of R2RML fluent configuration interfaces
    /// </summary>
    public interface IR2RMLConfiguration : IR2RML
    {
        /// <summary>
        /// Creates a <see cref="ITriplesMapConfiguration"/>, which will use a concrete table or view as datasource 
        /// as described on http://www.w3.org/TR/r2rml/#physical-tables
        /// </summary>
        /// <param name="tablename">Unquoted table name (any quotes and parentheses will be trimmed)</param>
        ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename);

        /// <summary>
        /// Creates a <see cref="ITriplesMapConfiguration"/>, which will use a SQL query as datasource
        /// as described on http://www.w3.org/TR/r2rml/#r2rml-views
        /// </summary>
        /// <param name="sqlQuery">a valid SQL query</param>
        ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery);
    }
}