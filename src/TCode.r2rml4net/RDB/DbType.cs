namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Enumeration of types valid for mapping to RDF
    /// </summary>
    /// <remarks>Refer to each enum member for corresponding Core SQL 2008 types. Note that for each non-standard RDBMS implementation there may be other types or they may have different meaning</remarks>
    public enum DbType
    {
        /// <summary>
        /// An undefined SQL type. Values will be interpreted as string literals
        /// </summary>
        Undefined,
        /// <summary>
        /// Any string type 
        /// </summary>
        /// <remarks>In Core SQL 2008: CHARACTER, CHARACTER VARYING, CHARACTER LARGE OBJECT, NATIONAL CHARACTER, NATIONAL CHARACTER VARYING, NATIONAL CHARACTER LARGE OBJECT</remarks>
        String,
        /// <summary>
        /// A binary type
        /// </summary>
        /// <remarks>In Core SQL 2008: BINARY, BINARY VARYING, BINARY LARGE OBJECT</remarks>
        Binary,
        /// <summary>
        /// A floating point type
        /// </summary>
        /// <remarks>In Core SQL 2008: FLOAT, REAL, DOUBLE PRECISION</remarks>
        FloatingPoint,
        /// <summary>
        /// An integer type
        /// </summary>
        /// <remarks>In Core SQL 2008: SMALLINT, INTEGER, BIGINT</remarks>
        Integer,
        /// <summary>
        /// A boolean type
        /// </summary>
        /// <remarks>In Core SQL 2008: BOOLEAN</remarks>
        Boolean,
        /// <summary>
        /// A date type
        /// </summary>
        /// <remarks>In Core SQL 2008: DATE</remarks>
        Date,
        /// <summary>
        /// A time type
        /// </summary>
        /// <remarks>In Core SQL 2008: TIME</remarks>
        Time,
        /// <summary>
        /// A datetime type
        /// </summary>
        /// <remarks>In Core SQL 2008: DATETIME</remarks>
        DateTime,
        /// <summary>
        /// A decimal type
        /// </summary>
        /// <remarks>In Core SQL 2008: NUMERIC, DECIMAL</remarks>
        Decimal
    }
}