using System;
using Moq;
using NUnit.Framework;
using DatabaseSchemaReader;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;

namespace TCode.r2rml4net.Tests.DatabaseSchemaReader
{
    [TestFixture]
    public class DatabaseSchemaAdapterCommonTests
    {
        private Mock<DatabaseReader> _databaseReader;
        private Mock<IColumnTypeMapper> _columnTypeMapper;
        private DatabaseSchemaAdapter _adapter;

        [SetUp]
        public void SetupAdapter()
        {
            _columnTypeMapper = new Mock<IColumnTypeMapper>();
            _databaseReader = new Mock<DatabaseReader>();

            _adapter = new DatabaseSchemaAdapter(_databaseReader.Object, _columnTypeMapper.Object);
        }

        [Test]
        public void UsesCoreSQL2008MapperByDefault()
        {
            // when
            _adapter = new DatabaseSchemaAdapter(_databaseReader.Object);

            // then
            Assert.AreEqual(typeof(CoreSQL2008ColumTypeMapper), _adapter.ColumnTypeMapper.GetType());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CannotBeInitializedWithNullReader()
        {
            new DatabaseSchemaAdapter(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CannotBeInitializedWithNullColumnMapper()
        {
            new DatabaseSchemaAdapter(_databaseReader.Object, null);
        }
    }
}
