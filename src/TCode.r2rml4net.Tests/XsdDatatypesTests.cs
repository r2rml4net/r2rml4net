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
using NUnit.Framework;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class XsdDatatypesTests
    {
        [TestCase(R2RMLType.Binary, "http://www.w3.org/2001/XMLSchema#hexBinary")]
        [TestCase(R2RMLType.Boolean, "http://www.w3.org/2001/XMLSchema#boolean")]
        [TestCase(R2RMLType.Date, "http://www.w3.org/2001/XMLSchema#date")]
        [TestCase(R2RMLType.DateTime, "http://www.w3.org/2001/XMLSchema#dateTime")]
        [TestCase(R2RMLType.Decimal, "http://www.w3.org/2001/XMLSchema#decimal")]
        [TestCase(R2RMLType.FloatingPoint, "http://www.w3.org/2001/XMLSchema#double")]
        [TestCase(R2RMLType.Integer, "http://www.w3.org/2001/XMLSchema#integer")]
        [TestCase(R2RMLType.Time, "http://www.w3.org/2001/XMLSchema#time")]
        public void ReturnsCorrectUrisForNonStringDatatypes(R2RMLType type, string expectedUri)
        {
            // when
            Uri uri = XsdDatatypes.GetDataType(type);

            // then
            Assert.IsNotNull(uri);
            Assert.AreEqual(expectedUri, uri.AbsoluteUri);
        }

        [TestCase(R2RMLType.String)]
        [TestCase(R2RMLType.Undefined)]
        public void ReturnsNullForStringOrUndefinedDatatypes(R2RMLType type)
        {
            Assert.IsNull(XsdDatatypes.GetDataType(type));
        }
    }
}
