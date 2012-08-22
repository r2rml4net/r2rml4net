using System;

namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// Interface for validating the <a href="http://www.w3.org/TR/r2rml/#dfn-sql-version-identifier">sql version identifier</a>
    /// </summary>
    public interface ISqlVersionValidator
    {
        /// <summary>
        /// Check wheather the <paramref name="sqlVersion"/> is valid
        /// </summary>
        /// <returns>true if sql version is valid</returns>
        bool SqlVersionIsValid(Uri sqlVersion);
    }
}