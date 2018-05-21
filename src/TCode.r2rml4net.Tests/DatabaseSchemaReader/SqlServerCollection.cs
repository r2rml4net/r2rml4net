using Xunit;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [CollectionDefinition("SQL Server")]
    public class SqlServerCollection : ICollectionFixture<SqlServerFixture>
    {
    }
}