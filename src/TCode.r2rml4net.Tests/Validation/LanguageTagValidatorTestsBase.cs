using NUnit.Framework;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Tests.Validation
{
    public abstract class LanguageTagValidatorTestsBase<T> where T : ILanguageTagValidator
    {
        private T _validator;

        [SetUp]
        public void Setup()
        {
            _validator = NewSimpleLanguageTagValidator();
        }

        protected abstract T NewSimpleLanguageTagValidator();

        [TestCase("pl")]
        [TestCase("pl-pl")]
        [TestCase("de-CH")]
        [TestCase("de-DE-1901")]
        [TestCase("es-419")]
        [TestCase("sl-IT-nedis")]
        [TestCase("en-US-boont")]
        [TestCase("mn-Cyrl-MN")]
        [TestCase("x-fr-CH")]
        [TestCase("en-GB-boont-r-extended-sequence-x-private")]
        [TestCase("sr-Cyrl")]
        [TestCase("sr-Latn")]
        [TestCase("hy-Latn-IT-arevela")]
        public void CanValidateValidLanguage(string langTag)
        {
            Assert.IsTrue(_validator.LanguageTagIsValid(langTag));
        }

        [TestCase("123")]
        [TestCase("english")]
        public void CanValidateInvalidLanguage(string langTag)
        {
            Assert.IsFalse(_validator.LanguageTagIsValid(langTag));
        }
    }
}