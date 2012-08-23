namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// Interface for validating language tags
    /// </summary>
    public interface ILanguageTagValidator
    {
        /// <summary>
        /// Check wheather the <paramref name="languageTag"/> is valid
        /// </summary>
        /// <returns>true if language tag is valid</returns>
        bool LanguageTagIsValid(string languageTag);
    }
}