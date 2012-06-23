using System;
using NUnit.Framework;
using DatabaseSchemaReader.DataSchema;
using System.Reflection;
using System.IO;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [TestFixture(Category = "Database")]
    public class SqlServerDatabaseSchemaAdapterTests : DatabaseSchemaAdapterTestsBase
    {
        protected override DatabaseReader SetupAdapter()
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

            return new DatabaseReader(conString, SqlType.SqlServer);
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
