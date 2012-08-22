using NUnit.Framework;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Mapping.Tests.DataSets
{
    [TestFixture]
    public class SimpleLanguageTagValidatorTests : LanguageTagValidatorTestsBase<SimpleLanguageTagValidator>
    {
        #region Overrides of LanguageTagValidatorTestsBase<SimpleLanguageTagValidator>

        protected override SimpleLanguageTagValidator NewSimpleLanguageTagValidator()
        {
            return new SimpleLanguageTagValidator();
        }

        #endregion
    }
}