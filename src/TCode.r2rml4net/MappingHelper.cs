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
using System.Text;
using TCode.r2rml4net.RDB;
using System.Linq;

namespace TCode.r2rml4net
{
    /// <summary>
    /// Utility helper class for generating and working with mappings
    /// </summary>
    public static class MappingHelper
    {
        private static readonly char[] AllowedChars = new[] {'-', '.', '_', '~'};

        private static readonly Tuple<int, int>[] UnicodeRanges = new[]
            {
                new Tuple<int, int>(0xA0, 0xD7FF), 
                new Tuple<int, int>(0xF900, 0xFDCF), 
                new Tuple<int, int>(0xFDF0, 0xFFEF), 
                new Tuple<int, int>(0x10000, 0x1FFFD), 
                new Tuple<int, int>(0x20000, 0x2FFFD), 
                new Tuple<int, int>(0x30000, 0x3FFFD), 
                new Tuple<int, int>(0x40000, 0x4FFFD), 
                new Tuple<int, int>(0x50000, 0x5FFFD), 
                new Tuple<int, int>(0x60000, 0x6FFFD), 
                new Tuple<int, int>(0x70000, 0x7FFFD), 
                new Tuple<int, int>(0x80000, 0x8FFFD), 
                new Tuple<int, int>(0x90000, 0x9FFFD), 
                new Tuple<int, int>(0xA0000, 0xAFFFD), 
                new Tuple<int, int>(0xB0000, 0xBFFFD), 
                new Tuple<int, int>(0xC0000, 0xCFFFD), 
                new Tuple<int, int>(0xD0000, 0xDFFFD), 
                new Tuple<int, int>(0xE0000, 0xEFFFD), 
            };

        /// <summary>
        /// Encodes a unescaped URI string as defined by <a href="http://www.w3.org/TR/r2rml/#RFC3987">RFC3987</a>
        /// </summary>
        public static string UrlEncode(string unescapedString)
        {
            StringBuilder encodedString = new StringBuilder();

            foreach (var character in unescapedString)
            {
                if(IsIUnreserved(character))
                {
                    encodedString.Append(character);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(new[] {character});
                    foreach (var octet in bytes)
                    {
                        encodedString.AppendFormat("%{0}", octet.ToString("X2"));
                    }
                }
            }

            return encodedString.ToString();
        }

        /// <summary>
        /// Gets a velue indicating whether the <paramref name="character"/> is in the 
        /// <a href="http://tools.ietf.org/html/rfc3987#section-2.2">iunreserved production</a> 
        /// in <a href="http://www.w3.org/TR/r2rml/#RFC3987">RFC3987</a>
        /// </summary>
        public static bool IsIUnreserved(char character)
        {
            return char.IsLetterOrDigit(character) ||
                   AllowedChars.Contains(character) ||
                   UnicodeRanges.Any(range => character >= range.Item1 && character < range.Item2);
        }

        /// <summary>
        /// Encloses and delimits a column name with braces for use in <a href="http://www.w3.org/TR/r2rml/#from-template">templates</a>
        /// </summary>
        public static string EncloseColumnName(string columnName)
        {
            return string.Format("{{{0}}}", DatabaseIdentifiersHelper.DelimitIdentifier(columnName));
        }
    }
}