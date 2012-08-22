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
        readonly ICollection<Uri> _identifiers = new Collection<Uri>
            {
                new Uri("http://www.w3.org/ns/r2rml#SQL2008"),
                new Uri("http://www.w3.org/ns/r2rml#Oracle"),
                new Uri("http://www.w3.org/ns/r2rml#MySQL"),
                new Uri("http://www.w3.org/ns/r2rml#MSSQLServer"),
                new Uri("http://www.w3.org/ns/r2rml#HSQLDB"),
                new Uri("http://www.w3.org/ns/r2rml#PostgreSQL"),
                new Uri("http://www.w3.org/ns/r2rml#DB2"),
                new Uri("http://www.w3.org/ns/r2rml#Informix"),
                new Uri("http://www.w3.org/ns/r2rml#Ingres"),
                new Uri("http://www.w3.org/ns/r2rml#Progress"),
                new Uri("http://www.w3.org/ns/r2rml#SybaseASE"),
                new Uri("http://www.w3.org/ns/r2rml#SybaseSQLAnywhere"),
                new Uri("http://www.w3.org/ns/r2rml#Virtuoso"),
                new Uri("http://www.w3.org/ns/r2rml#Firebird")
            };

        #region Implementation of ISqlVersionValidator

        /// <summary>
        /// Check wheather the <paramref name="sqlVersion"/> is valid
        /// </summary>
        /// <returns>true if sql version is valid</returns>
        public bool SqlVersionIsValid(Uri sqlVersion)
        {
            return _identifiers.Contains(sqlVersion);
        }

        #endregion
    }
}