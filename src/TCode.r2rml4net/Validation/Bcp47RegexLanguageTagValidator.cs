using System.Text.RegularExpressions;

namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// An implementation of <see cref="ILanguageTagValidator"/>, which uses a regular expression
    /// to check if the language tag is valid accoring to <a href="http://www.rfc-editor.org/rfc/bcp/bcp47.txt">BCP 47</a>
    /// </summary>
    /// <remarks>see <a href="http://schneegans.de/lv/">http://schneegans.de/lv/</a> for more info</remarks>
    public class Bcp47RegexLanguageTagValidator : ILanguageTagValidator
    {
        private static readonly object ClassLock = new object();
        #region Regex string
		private const string LanguageTagValidationRegexString = @"^
(
	(
		(
			(
				(?'language'
					[a-z]{2,3}
				)
				(-
					(?'extlang'
						[a-z]{3}
					)
				){0,3}
			)
			|
			(?'language'
				[a-z]{4}
			)
			|
			(?'language'
				[a-z]{5,8}
			)
		)

		(-(?'script'
			[a-z]{4}
		))?

		(-(?'region'
			[a-z]{2}
			|
			[0-9]{3}
		))?

		(-
			(?'variant'
				[a-z0-9]{5,8}
				|
				[0-9][a-z0-9]{3}
			)
		)*
		
		(-
			(?'extensions'
				[a-z0-9-[x]]
				(-
					[a-z0-9]{2,8}
				)+
			)
		)*
		
		(-
			x(- 
				(?'privateuse'
					[a-z0-9]{1,8}
				)
			)+
		)?
	)
	|
	(
		x(- 
			(?'privateuse'
				[a-z0-9]{1,8}
			)
		)+
	)
	|
	(?'grandfathered'
		(?'irregular'
			en-GB-oed |
			i-ami |
			i-bnn |
			i-default |
			i-enochian |
			i-hak |
			i-klingon |
			i-lux |
			i-mingo |
			i-navajo |
			i-pwn |
			i-tao |
			i-tay |
			i-tsu |
			sgn-BE-FR |
			sgn-BE-NL |
			sgn-CH-DE
		)
		|
		(?'regular'
			art-lojban |
			cel-gaulish |
			no-bok |
			no-nyn |
			zh-guoyu |
			zh-hakka |
			zh-min |
			zh-min-nan |
			zh-xiang
		)
	)
)
$"; 
	#endregion
        private static Regex _languageTagValidationRegex;

        private static Regex LanguageTagValidationRegex
        {
            get
            {
                lock (ClassLock)
                {
                    if(_languageTagValidationRegex == null)
                    {
                        lock(ClassLock)
                        {
                            _languageTagValidationRegex = new Regex(LanguageTagValidationRegexString, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                        }
                    }
                    return _languageTagValidationRegex;
                }
            }
        }

        #region Implementation of ILanguageTagValidator

        /// <summary>
        /// Check wheather the <paramref name="langugeTag"/> is valid
        /// </summary>
        /// <returns>true if language tag is valid</returns>
        public bool LanguageTagIsValid(string langugeTag)
        {
            return LanguageTagValidationRegex.IsMatch(langugeTag);
        }

        #endregion
    }
}