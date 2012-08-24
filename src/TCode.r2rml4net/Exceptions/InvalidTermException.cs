using System;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Exceptions
{
    public class InvalidTermException : Exception
    {
        public ITermMap TermMap { get; private set; }

        public InvalidTermException(ITermMap termMap, string reason)
            : base(string.Format("Cannot generate RDF term for '{0}'. {1}", termMap.Node, reason))
        {
            TermMap = termMap;
        }
    }
}