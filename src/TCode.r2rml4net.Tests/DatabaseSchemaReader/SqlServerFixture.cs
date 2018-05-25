using System;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using SqlLocalDb;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    public class SqlServerFixture : IDisposable
    {
        private readonly LocalDatabase _database;

        public DatabaseSchemaAdapter DatabaseSchema { get; }

        public SqlServerFixture()
        {
            string dbInitScript = Resourcer.Resource.AsString("TestDbScripts.SqlServer.sql");
            _database = new LocalDatabase();

            using (var connection = _database.GetConnection())
            {
                foreach (var commandText in dbInitScript.Split(new[] { "go", "GO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }

                DatabaseSchema = new DatabaseSchemaAdapter(new DatabaseReader(connection), new MSSQLServerColumTypeMapper());
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}