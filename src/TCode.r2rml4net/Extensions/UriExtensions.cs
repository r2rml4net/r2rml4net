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
using System.Reflection;

namespace TCode.r2rml4net.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Uri"/> class
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// A pretty dirty workaround to have slashes unescaped to avoid relative
        /// <see cref="Uri"/> resolution and simmilar malfunctions
        /// </summary>
        /// <remarks>See http://stackoverflow.com/questions/2320533/system-net-uri-with-urlencoded-characters</remarks>
        public static void LeaveDotsAndSlashesEscaped(this Uri uri)
        {
            const int unEscapeDotsAndSlashes = 0x2000000;
            FieldInfo fieldInfo = uri.GetType().GetField("m_Syntax", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new MissingFieldException("'m_Syntax' field not found");
            }

            object uriParser = fieldInfo.GetValue(uri);

            fieldInfo = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new MissingFieldException("'m_Flags' field not found");
            }

            object uriSyntaxFlags = fieldInfo.GetValue(uriParser);

            // Clear the flag that we don't want
            uriSyntaxFlags = (int)uriSyntaxFlags & ~unEscapeDotsAndSlashes;

            fieldInfo.SetValue(uriParser, uriSyntaxFlags);
        }
    }
}