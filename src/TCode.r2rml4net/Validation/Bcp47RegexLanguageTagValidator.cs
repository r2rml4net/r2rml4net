#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

using System.Text.RegularExpressions;

namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// An implementation of <see cref="ILanguageTagValidator"/>, which uses a regular expression
    /// to check if the language tag is valid accoring to <a href="http://www.rfc-editor.org/rfc/bcp/bcp47.txt">BCP 47</a>
    /// </summary>
    /// <remarks>see <a href="http://schneegans.de/lv/">http://schneegans.de/lv/</a> for more info</remarks>
    public class Bcp47RegexLanguageTagValidator : SimpleLanguageTagValidator
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
        /// Check wheather the <paramref name="languageTag"/> is valid
        /// </summary>
        /// <returns>true if language tag is valid</returns>
        public override bool LanguageTagIsValid(string languageTag)
        {
            return base.LanguageTagIsValid(languageTag) && LanguageTagValidationRegex.IsMatch(languageTag);
        }

        #endregion
    }
}