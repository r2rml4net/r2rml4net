using System;

namespace TCode.r2rml4net.RDF
{
    public interface ITermType
    {
        /// <summary>
        /// Returns term type set with configuration
        /// or a default value as described on http://www.w3.org/TR/r2rml/#dfn-term-type 
        /// </summary>
        Uri URI { get; }
    }
}