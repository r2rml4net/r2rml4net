using NUnit.Framework;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Tests.Validation
{
    [TestFixture]
    class Bcp47RegexLanguageTagValidatorTests : LanguageTagValidatorTestsBase<Bcp47RegexLanguageTagValidator>
    {
        #region Overrides of LanguageTagValidatorTestsBase<Bcp47RegexLanguageTagValidator>

        protected override Bcp47RegexLanguageTagValidator NewSimpleLanguageTagValidator()
        {
            return new Bcp47RegexLanguageTagValidator();
        }

        #endregion
    }
}