using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net
{
    public class InvalidTemplateException : InvalidTermException
    {
        public InvalidTemplateException(ITermMap termMap)
            : base(string.Format("Cannot generate RDF term for {0}. Template is missing", termMap.Node))
        {
        }
    }
}