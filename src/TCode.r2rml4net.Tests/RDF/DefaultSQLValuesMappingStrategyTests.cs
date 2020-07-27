#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using System.Runtime.InteropServices;
using Moq;
using Xunit;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Tests.RDF
{
    public class DefaultSQLValuesMappingStrategyTests
    {
        private const int ColumnIndex = 1;

        private readonly DefaultSQLValuesMappingStrategy _strategy;
        private readonly Mock<IDataRecord> _logicalRow;

        public DefaultSQLValuesMappingStrategyTests()
        {
            _strategy = new DefaultSQLValuesMappingStrategy(new MappingOptions());
            _logicalRow = new Mock<IDataRecord>();
        }

        [Theory]
        [InlineData("System.Int64", XsdDatatypes.Integer)]
        [InlineData("System.Int32", XsdDatatypes.Integer)]
        [InlineData("System.Int16", XsdDatatypes.Integer)]
        [InlineData("System.DateTime", XsdDatatypes.DateTime)]
        [InlineData("System.Single", XsdDatatypes.Double)]
        [InlineData("System.Double", XsdDatatypes.Double)]
        [InlineData("System.Decimal", XsdDatatypes.Decimal)]
        public void MapsDotnetTypes(string typeName, string uri)
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(Type.GetType(typeName));
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);

            // then
            Assert.NotNull(datatype);
            Assert.Equal(uri, datatype.AbsoluteUri);
        }

        [Fact]
        public void MapsDateToDate()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("DATE");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.NotNull(datatype);
            Assert.Equal(XsdDatatypes.Date, datatype.AbsoluteUri);
        }

        [Fact]
        public void MapsTimeToTime()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("TIME");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.NotNull(datatype);
            Assert.Equal(XsdDatatypes.Time, datatype.AbsoluteUri);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void EnsuresBooleanIsLowercase(bool value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetValue(It.IsAny<int>())).Returns(value.ToString());

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Boolean));

            // then
            Assert.NotNull(valueString);
            Assert.Equal(expected, valueString);
            _logicalRow.VerifyAll();
        }

        [Theory]
        [InlineData(-5.9, "-5.9E0")]
        [InlineData(+0.00014770215000, "1.4770215E-4")]
        [InlineData(01E+3, "1.0E3")]
        [InlineData(0, "0.0E0")]
        [InlineData(100.0, "1.0E2")]
        public void EnsuresProperyDoubleForm(double value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetDouble(ColumnIndex)).Returns(value);

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Double));

            // then
            Assert.NotNull(valueString);
            Assert.Equal(expected, valueString);
            _logicalRow.VerifyAll();
        }

        [Theory]
        [InlineData("2020-07-27", XsdDatatypes.Date)]
        [InlineData("2020-07-27T20:15:10", XsdDatatypes.DateTime)]
        public void AssumesUtcTimezoneForDatesAndTimes(string value, string type)
        {
            // given
            _logicalRow.Setup(row => row.GetDateTime(ColumnIndex))
                .Returns(DateTime.Parse(value));

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(type));

            // then
            Assert.NotNull(valueString);
            Assert.Equal(value, valueString);
        }
    }
}