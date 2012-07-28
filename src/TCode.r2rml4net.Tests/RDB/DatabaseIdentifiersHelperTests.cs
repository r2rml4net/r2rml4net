using NUnit.Framework;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Tests.RDB
{
    [TestFixture]
    public class DatabaseIdentifiersHelperTests
    {
        [TestCase("[Column]", "Column")]
        [TestCase("\"Column\"", "Column")]
        [TestCase("`Column`", "Column")]
        [TestCase("`Column with spaces and_underscored]", "Column with spaces and_underscored")]
        [TestCase("\"Column with spaces and_underscored\"", "Column with spaces and_underscored")]
        [TestCase("[Column with spaces and_underscored]", "Column with spaces and_underscored")]
        public void EcsapesSimpleColumnNames(string inputName, string expectedName)
        {
            Assert.AreEqual(expectedName, DatabaseIdentifiersHelper.GetColumnNameUnquoted(inputName));
        }

        //[TestCase("Column with \"quoted value\"", @"""Column ""with quotes""")]
        //public void EcsapesColumnNamesWithQuotes(string inputName, string expectedName)
        //{
        //    Assert.AreEqual(expectedName, DatabaseIdentifiersHelper.GetColumnNameUnquoted(inputName));
        //}
    }
}