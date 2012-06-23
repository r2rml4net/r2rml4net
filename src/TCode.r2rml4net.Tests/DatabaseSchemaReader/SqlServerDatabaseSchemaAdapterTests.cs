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

        [TestCase(DbType.Integer, "Long")]
        [TestCase(DbType.Integer, "Short")]
        [TestCase(DbType.Integer, "Integer")]
        [TestCase(DbType.Integer, "Tiny")]
        [TestCase(DbType.String, "UnicodeText")]
        [TestCase(DbType.String, "Text")]
        [TestCase(DbType.String, "FixedLength")]
        [TestCase(DbType.String, "UnicodeFixedLength")]
        [TestCase(DbType.Boolean, "Boolean")]
        [TestCase(DbType.Binary, "Binary")]
        [TestCase(DbType.Binary, "Image")]
        [TestCase(DbType.Binary, "Timestamp")]
        [TestCase(DbType.Date, "Date")]
        [TestCase(DbType.DateTime, "Datetime")]
        [TestCase(DbType.DateTime, "Datetime2")]
        [TestCase(DbType.Time, "Time")]
        [TestCase(DbType.Decimal, "Decimal")]
        [TestCase(DbType.FloatingPoint, "Float")]
        [TestCase(DbType.Decimal, "Money")]
        [TestCase(DbType.Undefined, "Guid")]
        [TestCase(DbType.String, "Char")]
        [TestCase(DbType.DateTime, "DatetimeOffset")]
        [TestCase(DbType.Undefined, "Geography")]
        [TestCase(DbType.Undefined, "Geometry")]
        [TestCase(DbType.Undefined, "Hierarchy")]
        [TestCase(DbType.String, "Nchar")]
        [TestCase(DbType.String, "Ntext")]
        [TestCase(DbType.Decimal, "Numeric")]
        [TestCase(DbType.DateTime, "Smalldatetime")]
        [TestCase(DbType.Undefined, "SqlVariant")]
        [TestCase(DbType.String, "Text")]
        [TestCase(DbType.Binary, "Varbinary")]
        [TestCase(DbType.String, "XML")]
        public void CorrectlyMapsSqlTypes(DbType dbType, string columnName)
        {
            // when
            TableMetadata table = DatabaseSchema.Tables["ManyDataTypes"];

            // then
            Assert.AreEqual(35, table.ColumnsCount, "Column count mismatch. Some columns not tested");
            Assert.AreEqual(dbType, table[columnName].Type);
        }
    }
}
