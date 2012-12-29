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

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Interface for creating configuration of term maps as described on http://www.w3.org/TR/r2rml/#term-map
    /// </summary>
    public interface ITermMapConfiguration : ITermMap
    {
        /// <summary>
        /// Allows setting the term type of the current term map
        /// </summary>
        /// <remarks>Please see <see cref="ITermTypeConfiguration"/> members for info on valid values for different term maps</remarks>
        new ITermTypeConfiguration TermType { get; }

        /// <summary>
        /// Sets the term map as constant-valued
        /// </summary>
        /// <param name="uri">Constant value URI</param>
        /// <remarks>
        /// Asserted using the rr:constant property described on http://www.w3.org/TR/r2rml/#constant
        /// </remarks>
        ITermTypeConfiguration IsConstantValued(Uri uri);

        /// <summary>
        /// Sets the term map as template-values
        /// </summary>
        /// <param name="template">A valid template</param>
        /// <remarks>
        /// For details on templates read more on http://www.w3.org/TR/r2rml/#from-template
        /// </remarks>
        ITermTypeConfiguration IsTemplateValued(string template);

        /// <summary>
        /// Sets the term map's inverse expression template
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#inverse</remarks>
        ITermMapConfiguration SetInverseExpression(string stringTemplate);
    }
}
