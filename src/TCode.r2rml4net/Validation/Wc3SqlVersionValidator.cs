#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
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
        private readonly ICollection<string> _identifiers = new Collection<string>
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