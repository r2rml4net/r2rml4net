namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration of object maps
    /// </summary>
    public interface IObjectMapConfiguration : ITermMapConfiguration
    {
        /// <summary>
        /// Sets the object map as constant-valued
        /// </summary>
        /// <param name="literal">Constant value literal</param>
        /// <remarks>
        /// Asserted using the rr:constant property as described on http://www.w3.org/TR/r2rml/#constant
        /// </remarks>
        ILiteralTermMapConfiguration IsConstantValued(string literal);
        /// <summary>
        /// Sets the object map as column-valued
        /// </summary>
        /// <param name="columnName">Column name</param>
        ILiteralTermMapConfiguration IsColumnValued(string columnName);
    }
}