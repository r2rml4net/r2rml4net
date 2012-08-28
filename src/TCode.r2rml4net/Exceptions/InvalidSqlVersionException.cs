using System;

namespace TCode.r2rml4net.Exceptions
{
    /// <summary>
    /// Represents errors occuring when a <a href="http://www.w3.org/TR/r2rml/#logical-tables">logical table</a> 
    /// has an invalid <a href="http://www.w3.org/TR/r2rml/#dfn-sql-version-identifier">SQL version identifier</a>
    /// </summary>
    public class InvalidSqlVersionException : InvalidMapException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidSqlVersionException"/> with the invalid <paramref name="uri"/>
        /// </summary>
        public InvalidSqlVersionException(Uri uri)
            : base(string.Format("Sql version {0} is invalid", uri))
        {
        }
    }
}