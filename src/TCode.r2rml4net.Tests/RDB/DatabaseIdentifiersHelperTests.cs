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

        [TestCase('[', ']')]
        [TestCase('`', '`')]
        [TestCase('\"', '\"')]
        public void DoesntDelimitIfAlreadyDelimited(char delimitLeft, char delimitRight)
        {
            // given
            string sqlId = string.Format("{0}some idenfifier{1}", delimitLeft, delimitRight);

            // when
            var delimited = DatabaseIdentifiersHelper.DelimitIdentifier(sqlId, new MappingOptions());

            // then
            Assert.AreEqual(sqlId, delimited);
        }
    }
}