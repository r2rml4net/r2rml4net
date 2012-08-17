using NUnit.Framework;
using TCode.r2rml4net.Mapping.DataSets;

namespace TCode.r2rml4net.Mapping.Tests.DataSets
{
    [TestFixture]
    public class LangagesTests
    {
        [TestCase("pl")]
        [TestCase("pl-pl")]
        [TestCase("sl-Latn-IT-rozaj")]
        public void CanValidateValidLanguage(string langTag)
        {
            Assert.IsTrue(Languages.LanguageTagIsValid(langTag));
        }

        [TestCase("123")]
        [TestCase("english")]
        public void CanValidateInvalidLanguage(string langTag)
        {
            Assert.IsFalse(Languages.LanguageTagIsValid(langTag));
        }
    }
}