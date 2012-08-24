using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// Implementation of <see cref="ISqlVersionValidator"/>, which check
    /// whether the identifier is on the
    /// <a href="http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs">non-normative list of identifiers for other SQL versions</a>
    /// </summary>
    public class Wc3SqlVersionValidator : ISqlVersionValidator
    {
        readonly ICollection<string> _identifiers = new Collection<string>
            {
                "http://www.w3.org/ns/r2rml#SQL2008",
                "http://www.w3.org/ns/r2rml#Oracle",
                "http://www.w3.org/ns/r2rml#MySQL",
                "http://www.w3.org/ns/r2rml#MSSQLServer",
                "http://www.w3.org/ns/r2rml#HSQLDB",
                "http://www.w3.org/ns/r2rml#PostgreSQL",
                "http://www.w3.org/ns/r2rml#DB2",
                "http://www.w3.org/ns/r2rml#Informix",
                "http://www.w3.org/ns/r2rml#Ingres",
                "http://www.w3.org/ns/r2rml#Progress",
                "http://www.w3.org/ns/r2rml#SybaseASE",
                "http://www.w3.org/ns/r2rml#SybaseSQLAnywhere",
                "http://www.w3.org/ns/r2rml#Virtuoso",
                "http://www.w3.org/ns/r2rml#Firebird"
            };

        #region Implementation of ISqlVersionValidator

        /// <summary>
        /// Check wheather the <paramref name="sqlVersion"/> is valid
        /// </summary>
        /// <returns>true if sql version is valid</returns>
        public bool SqlVersionIsValid(Uri sqlVersion)
        {
            return _identifiers.Contains(sqlVersion.ToString());
        }

        #endregion
    }
}