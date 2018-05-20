#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using Xunit;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Tests.Validation
{
    public abstract class LanguageTagValidatorTestsBase<T> where T : ILanguageTagValidator
    {
        private readonly T _validator;

        protected LanguageTagValidatorTestsBase()
        {
            _validator = NewSimpleLanguageTagValidator();
        }

        protected abstract T NewSimpleLanguageTagValidator();

        [Theory]
        [InlineData("pl")]
        [InlineData("pl-pl")]
        [InlineData("de-CH")]
        [InlineData("de-DE-1901")]
        [InlineData("es-419")]
        [InlineData("sl-IT-nedis")]
        [InlineData("en-US-boont")]
        [InlineData("mn-Cyrl-MN")]
        [InlineData("x-fr-CH")]
        [InlineData("en-GB-boont-r-extended-sequence-x-private")]
        [InlineData("sr-Cyrl")]
        [InlineData("sr-Latn")]
        [InlineData("hy-Latn-IT-arevela")]
        public void CanValidateValidLanguage(string langTag)
        {
            Assert.True(_validator.LanguageTagIsValid(langTag));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("english")]
        public void CanValidateInvalidLanguage(string langTag)
        {
            Assert.False(_validator.LanguageTagIsValid(langTag));
        }
    }
}