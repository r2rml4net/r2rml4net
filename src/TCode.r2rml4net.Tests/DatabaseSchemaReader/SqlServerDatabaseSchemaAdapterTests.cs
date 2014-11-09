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
using DatabaseSchemaReader.DataSchema;
using System.Reflection;
using System.IO;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [TestFixture(Category = "Database")]
    public class SqlServerDatabaseSchemaAdapterTests : DatabaseSchemaAdapterTestsBase
    {
        protected override DatabaseSchemaAdapter SetupAdapter()
        {
            string dbInitScript;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TCode.r2rml4net.Tests.DatabaseSchemaReader.TestDbScripts.SqlServer.sql");
            using (StreamReader reader = new StreamReader(stream))
            {
                dbInitScript = reader.ReadToEnd();
            }

            var conStringMaster = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString;
            var conString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString;
            using (var connection = System.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                connection.ConnectionString = conStringMaster;
                connection.Open();

                foreach (var commandText in dbInitScript.Split(new[] { "go", "GO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return new DatabaseSchemaAdapter(new DatabaseReader(conString, SqlType.SqlServer), new MSSQLServerColumTypeMapper());
        }

        [TestCase(R2RMLType.Integer, "Long")]
        [TestCase(R2RMLType.Integer, "Short")]
        [TestCase(R2RMLType.Integer, "Integer")]
        [TestCase(R2RMLType.Integer, "Tiny")]
        [TestCase(R2RMLType.String, "UnicodeText")]
        [TestCase(R2RMLType.String, "Text")]
        [TestCase(R2RMLType.String, "FixedLength")]
        [TestCase(R2RMLType.String, "UnicodeFixedLength")]
        [TestCase(R2RMLType.Boolean, "Boolean")]
        [TestCase(R2RMLType.Binary, "Binary")]
        [TestCase(R2RMLType.Binary, "Image")]
        [TestCase(R2RMLType.Binary, "Timestamp")]
        [TestCase(R2RMLType.Date, "Date")]
        [TestCase(R2RMLType.DateTime, "Datetime")]
        [TestCase(R2RMLType.DateTime, "Datetime2")]
        [TestCase(R2RMLType.Time, "Time")]
        [TestCase(R2RMLType.Decimal, "Decimal")]
        [TestCase(R2RMLType.FloatingPoint, "Float")]
        [TestCase(R2RMLType.Decimal, "Money")]
        [TestCase(R2RMLType.Undefined, "Guid")]
        [TestCase(R2RMLType.String, "Char")]
        [TestCase(R2RMLType.DateTime, "DatetimeOffset")]
        [TestCase(R2RMLType.Undefined, "Geography")]
        [TestCase(R2RMLType.Undefined, "Geometry")]
        [TestCase(R2RMLType.Undefined, "Hierarchy")]
        [TestCase(R2RMLType.String, "Nchar")]
        [TestCase(R2RMLType.String, "Ntext")]
        [TestCase(R2RMLType.Decimal, "Numeric")]
        [TestCase(R2RMLType.DateTime, "Smalldatetime")]
        [TestCase(R2RMLType.Undefined, "SqlVariant")]
        [TestCase(R2RMLType.String, "Text")]
        [TestCase(R2RMLType.Binary, "Varbinary")]
        [TestCase(R2RMLType.String, "XML")]
        public void CorrectlyMapsSqlTypes(R2RMLType r2RMLType, string columnName)
        {
            // when
            TableMetadata table = DatabaseSchema.Tables["ManyDataTypes"];

            // then
            Assert.AreEqual(35, table.ColumnsCount, "Column count mismatch. Some columns not tested");
            Assert.AreEqual(r2RMLType, table[columnName].Type);
        }
    }
}
