using System;
using System.Globalization;

namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface ILiteralTermMapConfiguration
    {
        void HasDataType(Uri dataTypeUri);
        void HasDataType(string dataTypeUri);
        void HasLanguageTag(string languagTag);
        void HasLanguageTag(CultureInfo cultureInfo);
    }
}