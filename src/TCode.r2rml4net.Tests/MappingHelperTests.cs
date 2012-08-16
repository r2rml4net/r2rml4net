using NUnit.Framework;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class MappingHelperTests
    {
        [Test]
        public void EnsuresUppercaseEscapedOctets()
        {
            // given
            const string unescaped = "some, text; with: illegal/ characters";

            // when
            string escaped = new MappingHelper(new MappingOptions()).UrlEncode(unescaped);

            // then
            Assert.AreEqual("some%2C%20text%3B%20with%3A%20illegal%2F%20characters", escaped);
        }

        [TestCase(".~_-")]
        [TestCase("abcdefghigklmnopqrstuvwxyz")]
        [TestCase("QWERTYUIOPASFFGHJKLZXCVBNM")]
        [TestCase("0123654789")]
        public void DoesntEncodeAllowedCharcters(string character)
        {
            Assert.AreEqual(character, new MappingHelper(new MappingOptions()).UrlEncode(character));
        }

        [TestCase(" ", "%20")]
        [TestCase(",", "%2C")]
        [TestCase(";", "%3B")]
        [TestCase(":", "%3A")]
        [TestCase("/", "%2F")]
        [TestCase("/(..)/", "%2F%28..%29%2F")]
        public void EncodesCharactersCaseSensitive(string character, string expectedEncoded)
        {
            Assert.AreEqual(expectedEncoded, new MappingHelper(new MappingOptions()).UrlEncode(character));
        }

        [TestCase("成")]
        [TestCase("用")]
        [TestCase("カタカ")]
        public void DoesntEscapeEasterScript(string character)
        {
            Assert.AreEqual(character, new MappingHelper(new MappingOptions()).UrlEncode(character));
            
        }
    }
}