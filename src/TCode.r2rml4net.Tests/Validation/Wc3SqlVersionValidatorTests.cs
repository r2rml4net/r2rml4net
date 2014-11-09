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
using NUnit.Framework;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Tests.Validation
{
    [TestFixture]
    public class Wc3SqlVersionValidatorTests
    {
        private Wc3SqlVersionValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new Wc3SqlVersionValidator();
        }

        [TestCase("http://www.w3.org/ns/r2rml#SQL2008")]
        [TestCase("http://www.w3.org/ns/r2rml#Oracle")]
        [TestCase("http://www.w3.org/ns/r2rml#MySQL")]
        [TestCase("http://www.w3.org/ns/r2rml#MSSQLServer")]
        [TestCase("http://www.w3.org/ns/r2rml#HSQLDB")]
        [TestCase("http://www.w3.org/ns/r2rml#PostgreSQL")]
        [TestCase("http://www.w3.org/ns/r2rml#DB2")]
        [TestCase("http://www.w3.org/ns/r2rml#Informix")]
        [TestCase("http://www.w3.org/ns/r2rml#Ingres")]
        [TestCase("http://www.w3.org/ns/r2rml#Progress")]
        [TestCase("http://www.w3.org/ns/r2rml#SybaseASE")]
        [TestCase("http://www.w3.org/ns/r2rml#SybaseSQLAnywhere")]
        [TestCase("http://www.w3.org/ns/r2rml#Virtuoso")]
        [TestCase("http://www.w3.org/ns/r2rml#Firebird")]
        public void ReturnsTrueForValidLanguageTags(string tagString)
        {
            Uri tag = new Uri(tagString);
            Assert.IsTrue(_validator.SqlVersionIsValid(tag));
        }

        [Test]
        public void ReturnsFalseForInvalidLanguageTags()
        {
            Uri tag = new Uri("http://www.w3.org/ns/r2rml#Firebird");
            Assert.IsTrue(_validator.SqlVersionIsValid(tag));
        }
    }
}