#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Types used by R2RML mappings as described on http://www.w3.org/TR/r2rml/#natural-mapping
    /// </summary>
    public class XsdDatatypes
    {
        /// <summary>
        /// Gets full URI string for xsd:integer
        /// </summary>
        public const string Integer = "http://www.w3.org/2001/XMLSchema#integer";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#boolean
        /// </summary>
        public const string Boolean = "http://www.w3.org/2001/XMLSchema#boolean";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#decimal
        /// </summary>
        public const string Decimal = "http://www.w3.org/2001/XMLSchema#decimal";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#double
        /// </summary>
        public const string Double = "http://www.w3.org/2001/XMLSchema#double";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#date
        /// </summary>
        public const string Date = "http://www.w3.org/2001/XMLSchema#date";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#time
        /// </summary>
        public const string Time = "http://www.w3.org/2001/XMLSchema#time";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#dateTime
        /// </summary>
        public const string DateTime = "http://www.w3.org/2001/XMLSchema#dateTime";

        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#hexBinary
        /// </summary>
        public const string Binary = "http://www.w3.org/2001/XMLSchema#hexBinary";

        /// <summary>
        /// Get a xsd type URI for the given <see cref="R2RMLType"/>
        /// </summary>
        /// <returns>a URI or null for string/undefined type</returns>
        /// <remarks>Read more on http://www.w3.org/TR/r2rml/#natural-mapping</remarks>
        public static Uri GetDataType(R2RMLType columnType)
        {
            switch (columnType)
            {
                case R2RMLType.Binary:
                    return new Uri(Binary);
                case R2RMLType.Integer:
                    return new Uri(Integer);
                case R2RMLType.Decimal:
                    return new Uri(Decimal);
                case R2RMLType.FloatingPoint:
                    return new Uri(Double);
                case R2RMLType.Date:
                    return new Uri(Date);
                case R2RMLType.Time:
                    return new Uri(Time);
                case R2RMLType.DateTime:
                    return new Uri(DateTime);
                case R2RMLType.Boolean:
                    return new Uri(Boolean);
                default:
                    return null;
            }
        }
    }
}
