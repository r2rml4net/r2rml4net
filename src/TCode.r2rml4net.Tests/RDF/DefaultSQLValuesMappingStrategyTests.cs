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
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Tests.RDF
{
    [TestFixture]
    public class DefaultSQLValuesMappingStrategyTests
    {
        private const int ColumnIndex = 1;

        private DefaultSQLValuesMappingStrategy _strategy;
        private Mock<IDataRecord> _logicalRow;

        [SetUp]
        public void Setup()
        {
            _strategy = new DefaultSQLValuesMappingStrategy();
            _logicalRow = new Mock<IDataRecord>();
        }

        [TestCase("System.Int64", XsdDatatypes.Integer)]
        [TestCase("System.Int32", XsdDatatypes.Integer)]
        [TestCase("System.Int16", XsdDatatypes.Integer)]
        [TestCase("System.DateTime", XsdDatatypes.DateTime)]
        [TestCase("System.Single", XsdDatatypes.Double)]
        [TestCase("System.Double", XsdDatatypes.Double)]
        [TestCase("System.Decimal", XsdDatatypes.Decimal)] 
        public void MapsDotnetTypes(string typeName, string uri)
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(Type.GetType(typeName));
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);

            // then
            Assert.IsNotNull(datatype);
            Assert.AreEqual(uri, datatype.AbsoluteUri);
        }

        [Test]
        public void MapsDateToDate()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("DATE");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.IsNotNull(datatype);
            Assert.AreEqual(XsdDatatypes.Date, datatype.AbsoluteUri);
        }

        [Test]
        public void MapsTimeToTime()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("TIME");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.IsNotNull(datatype);
            Assert.AreEqual(XsdDatatypes.Time, datatype.AbsoluteUri);
        }

        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void EnsuresBooleanIsLowercase(bool value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetValue(It.IsAny<int>())).Returns(value.ToString());

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Boolean));

            // then
            Assert.IsNotNull(valueString);
            Assert.AreEqual(expected, valueString);
            _logicalRow.VerifyAll();
        }

        [TestCase(-5.9, "-5.9E0")]
        [TestCase(+0.00014770215000, "1.4770215E-4")]
        [TestCase(01E+3, "1.0E3")]
        [TestCase(0, "0.0E0")]
        [TestCase(100.0, "1.0E2")]
        public void EnsuresProperyDoubleForm(double value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetDouble(ColumnIndex)).Returns(value);

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Double));

            // then
            Assert.IsNotNull(valueString);
            Assert.AreEqual(expected, valueString);
            _logicalRow.VerifyAll();
        }
    }
}