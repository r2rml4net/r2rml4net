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
using Xunit;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [Collection("SQL Server")]
    [Trait("Category", "Database")]
    public class SqlServerDatabaseSchemaAdapterTests : DatabaseSchemaAdapterTestsBase
    {
        public SqlServerDatabaseSchemaAdapterTests(SqlServerFixture fixture) : base(fixture)
        {
        }

        [Theory]
        [InlineData(R2RMLType.Integer, "Long")]
        [InlineData(R2RMLType.Integer, "Short")]
        [InlineData(R2RMLType.Integer, "Integer")]
        [InlineData(R2RMLType.Integer, "Tiny")]
        [InlineData(R2RMLType.String, "UnicodeText")]
        [InlineData(R2RMLType.String, "Text")]
        [InlineData(R2RMLType.String, "FixedLength")]
        [InlineData(R2RMLType.String, "UnicodeFixedLength")]
        [InlineData(R2RMLType.Boolean, "Boolean")]
        [InlineData(R2RMLType.Binary, "Binary")]
        [InlineData(R2RMLType.Binary, "Image")]
        [InlineData(R2RMLType.Binary, "Timestamp")]
        [InlineData(R2RMLType.Date, "Date")]
        [InlineData(R2RMLType.DateTime, "Datetime")]
        [InlineData(R2RMLType.DateTime, "Datetime2")]
        [InlineData(R2RMLType.Time, "Time")]
        [InlineData(R2RMLType.Decimal, "Decimal")]
        [InlineData(R2RMLType.FloatingPoint, "Float")]
        [InlineData(R2RMLType.Decimal, "Money")]
        [InlineData(R2RMLType.Undefined, "Guid")]
        [InlineData(R2RMLType.String, "Char")]
        [InlineData(R2RMLType.DateTime, "DatetimeOffset")]
        [InlineData(R2RMLType.Undefined, "Geography")]
        [InlineData(R2RMLType.Undefined, "Geometry")]
        [InlineData(R2RMLType.Undefined, "Hierarchy")]
        [InlineData(R2RMLType.String, "Nchar")]
        [InlineData(R2RMLType.String, "Ntext")]
        [InlineData(R2RMLType.Decimal, "Numeric")]
        [InlineData(R2RMLType.DateTime, "Smalldatetime")]
        [InlineData(R2RMLType.Undefined, "SqlVariant")]
        [InlineData(R2RMLType.Binary, "Varbinary")]
        [InlineData(R2RMLType.String, "XML")]
        public void CorrectlyMapsSqlTypes(R2RMLType r2RMLType, string columnName)
        {
            // when
            TableMetadata table = DatabaseSchema.Tables["ManyDataTypes"];

            // then
            Assert.Equal(35, table.ColumnsCount);
            Assert.Equal(r2RMLType, table[columnName].Type);
        }
    }
}
