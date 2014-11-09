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

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a term map
    /// </summary>
    /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-term-map</remarks>
    public interface ITermMap : IMapBase
    {
        /// <summary>
        /// Gets template or null if absent
        /// </summary>
        string Template { get; }

        /// <summary>
        /// Gets the term type set with configuration
        /// or a default value
        /// </summary>
        /// <remarks>Default value is described on http://www.w3.org/TR/r2rml/#dfn-term-type</remarks>
        Uri TermTypeURI { get; }

        /// <summary>
        /// Gets column or null if not set
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// Gets the inverse expression associated with this <see cref="ITermMap"/> or null if not set
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#inverse</remarks>
        string InverseExpression { get; }

        /// <summary>
        /// Gets a value indicating whether <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a> 
        /// is <a href="http://www.w3.org/TR/r2rml/#constant">constant valued</a>
        /// </summary>
        bool IsConstantValued { get; }

        /// <summary>
        /// Gets a value indicating whether <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a> 
        /// is <a href="http://www.w3.org/TR/r2rml/#from-column">column valued</a>
        /// </summary>
        bool IsColumnValued { get; }

        /// <summary>
        /// Gets a value indicating whether <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a> 
        /// is <a href="http://www.w3.org/TR/r2rml/#from-template">template valued</a>
        /// </summary>
        bool IsTemplateValued { get; }

        /// <summary>
        /// Gets <a href="http://www.w3.org/TR/r2rml/#term-map">term map's</a> 
        /// <a href="http://www.w3.org/TR/r2rml/#termtype">term type</a>
        /// </summary>
        ITermType TermType { get; }
    }
}