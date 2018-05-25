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

namespace TCode.r2rml4net.Tests
{
    public class MappingHelperTests
    {
        [Fact]
        public void EnsuresUppercaseEscapedOctets()
        {
            // given
            const string unescaped = "some, text; with: illegal/ characters";

            // when
            string escaped = MappingHelper.UrlEncode(unescaped);

            // then
            Assert.Equal("some%2C%20text%3B%20with%3A%20illegal%2F%20characters", escaped);
        }

        [Theory]
        [InlineData(".~_-")]
        [InlineData("abcdefghigklmnopqrstuvwxyz")]
        [InlineData("QWERTYUIOPASFFGHJKLZXCVBNM")]
        [InlineData("0123654789")]
        public void DoesntEncodeAllowedCharcters(string character)
        {
            Assert.Equal(character, MappingHelper.UrlEncode(character));
        }

        [Theory]
        [InlineData(" ", "%20")]
        [InlineData(",", "%2C")]
        [InlineData(";", "%3B")]
        [InlineData(":", "%3A")]
        [InlineData("/", "%2F")]
        [InlineData("/(..)/", "%2F%28..%29%2F")]
        public void EncodesCharactersCaseSensitive(string character, string expectedEncoded)
        {
            Assert.Equal(expectedEncoded, MappingHelper.UrlEncode(character));
        }

        [Theory]
        [InlineData("成")]
        [InlineData("用")]
        [InlineData("カタカ")]
        public void DoesntEscapeEasterScript(string character)
        {
            Assert.Equal(character, MappingHelper.UrlEncode(character));
            
        }
    }
}