namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Configuration of term maps, which cannot be of term type rr:Literal. See http://www.w3.org/TR/r2rml/#termtype
    /// </summary>
    public interface INonLiteralTermMapConfigutarion
    {
        /// <summary>
        /// Sets term map to as column-valued
        /// </summary>
        /// <param name="columnName">column name</param>
        void IsColumnValued(string columnName);
    }
}