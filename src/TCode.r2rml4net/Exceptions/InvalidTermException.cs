using System;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Exceptions
{
    /// <summary>
    /// Represents errors occuring during <a href="http://www.w3.org/TR/r2rml/#generated-rdf-term">RDF term generation</a>
    /// </summary>
    public class InvalidTermException : Exception
    {
        /// <summary>
        /// The <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>, 
        /// which cannot be used to <a href="http://www.w3.org/TR/r2rml/#generated-rdf-term">generate an RDF term</a>
        /// </summary>
        public ITermMap TermMap { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="InvalidTermException"/> for a <paramref name="termMap"/>
        /// with a given <paramref name="reason"/> why it occured
        /// </summary>
        public InvalidTermException(ITermMap termMap, string reason)
            : base(string.Format("Cannot generate RDF term for '{0}'. {1}", termMap.Node, reason))
        {
            TermMap = termMap;
        }
    }
}