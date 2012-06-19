using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DatabaseSchemaReader.DataSchema;
using System.Reflection;
using System.IO;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [TestFixture]
    public class DatabaseSchemaAdapterTests
    {
        [Test]
        public void TestSqlServer()
        {
            string dbInitScript;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TCode.r2rml4net.Tests.DatabaseSchemaReader.TestDbScripts.SqlServer.sql");
            using (StreamReader reader = new StreamReader(stream))
            {
                dbInitScript = reader.ReadToEnd();
            }

            var conString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString;
            using (var connection = System.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                connection.ConnectionString = conString;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = dbInitScript;
                command.ExecuteNonQuery();
                connection.Close();
            }

            DatabaseSchema _schema = new DatabaseSchema(conString, SqlType.SqlServer);
        }
    }
}
