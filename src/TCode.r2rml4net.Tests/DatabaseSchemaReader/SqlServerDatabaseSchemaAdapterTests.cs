using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DatabaseSchemaReader.DataSchema;
using System.Reflection;
using System.IO;
using DatabaseSchemaReader;

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
    }
}
