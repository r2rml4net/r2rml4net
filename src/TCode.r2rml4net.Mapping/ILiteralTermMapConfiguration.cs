using System;
using System.Globalization;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration for term maps which can be of term-type rr:Literal. See http://www.w3.org/TR/r2rml/#termtype
    /// </summary>
    public interface ILiteralTermMapConfiguration
    {
        /// <summary>
        /// Sets the datatype of the term map
        /// </summary>
        /// <remarks>language tag and datatype cannot be set simultaneously</remarks>
        void HasDataType(Uri dataTypeUri);
        /// <summary>
        /// Sets the datatype of the term map
        /// </summary>
        /// <remarks>language tag and datatype cannot be set simultaneously</remarks>
        void HasDataType(string dataTypeUri);
        /// <summary>
        /// Sets the language tag of the term map
        /// </summary>
        /// <remarks>language tag and datatype cannot be set simultaneously</remarks>
        void HasLanguageTag(string languagTag);
        /// <summary>
        /// Sets the language tag of the term map
        /// </summary>
        /// <remarks>language tag and datatype cannot be set simultaneously</remarks>
        void HasLanguageTag(CultureInfo cultureInfo);
    }
}