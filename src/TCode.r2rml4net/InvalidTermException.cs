using System;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net
{
    public class InvalidTermException : Exception
    {
        private const string InvalidTermValueFormatString = "Cannot generate RDF term for {0}. It produces an invalid value {1}";

        public InvalidTermException(ITermMap termMap, string invalidValue) 
            : base(string.Format(InvalidTermValueFormatString, termMap.Node, invalidValue))
        {
        }

        public InvalidTermException(ITermMap termMap)
            : base(string.Format("Cannot generate RDF term for {0}", termMap.Node))
        {
        }
    }
}