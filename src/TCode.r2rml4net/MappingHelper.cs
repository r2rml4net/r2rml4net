using System;
using System.Text;
using TCode.r2rml4net.RDB;
using System.Linq;

namespace TCode.r2rml4net
{
    public class MappingHelper
    {
        private static readonly char[] AllowedChars = new[] {'-', '.', '_', '~'};

        private static readonly Tuple<int, int>[] UnicodeRanges = new[]
            {
                new Tuple<int, int>(0xA0, 0xD7FF), 
                new Tuple<int, int>(0xF900, 0xFDCF), 
                new Tuple<int, int>(0xFDF0, 0xFFEF), 
                new Tuple<int, int>(0x10000, 0x1FFFD), 
                new Tuple<int, int>(0x20000, 0x2FFFD), 
                new Tuple<int, int>(0x30000, 0x3FFFD), 
                new Tuple<int, int>(0x40000, 0x4FFFD), 
                new Tuple<int, int>(0x50000, 0x5FFFD), 
                new Tuple<int, int>(0x60000, 0x6FFFD), 
                new Tuple<int, int>(0x70000, 0x7FFFD), 
                new Tuple<int, int>(0x80000, 0x8FFFD), 
                new Tuple<int, int>(0x90000, 0x9FFFD), 
                new Tuple<int, int>(0xA0000, 0xAFFFD), 
                new Tuple<int, int>(0xB0000, 0xBFFFD), 
                new Tuple<int, int>(0xC0000, 0xCFFFD), 
                new Tuple<int, int>(0xD0000, 0xDFFFD), 
                new Tuple<int, int>(0xE0000, 0xEFFFD), 
            };
        private readonly MappingOptions _options;

        public MappingHelper(MappingOptions options)
        {
            _options = options;
        }

        public string UrlEncode(string unescapedString)
        {
            StringBuilder encodedString = new StringBuilder();

            foreach (var character in unescapedString)
            {
                if(IsIUnreserved(character))
                {
                    encodedString.Append(character);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(new[] {character});
                    foreach (var octet in bytes)
                    {
                        encodedString.AppendFormat("%{0}", octet.ToString("X2"));
                    }
                }
            }

            return encodedString.ToString();
        }

        private static bool IsIUnreserved(char character)
        {
            return char.IsLetterOrDigit(character) ||
                   AllowedChars.Contains(character) ||
                   UnicodeRanges.Any(range => character >= range.Item1 && character < range.Item2);
        }

        public virtual string EncloseColumnName(string columnName)
        {
            return string.Format("{{{0}}}", DatabaseIdentifiersHelper.DelimitIdentifier(columnName, _options));
        }
    }
}