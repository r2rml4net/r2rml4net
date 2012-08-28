using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Exceptions
{
    /// <summary>
    /// Represents errors occuring, when <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
    /// has missing template
    /// </summary>
    public class InvalidTemplateException : InvalidTermException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidTemplateException"/> for a <paramref name="termMap"/>
        /// </summary>
        public InvalidTemplateException(ITermMap termMap)
            : base(termMap, "Template is missing")
        {
        }
    }
}