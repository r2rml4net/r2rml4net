using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a R2RML mapping
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/</remarks>
    public interface IR2RML
    {
        /// <summary>
        /// Gets triple maps contained by this <see cref="IR2RML"/>
        /// </summary>
        IEnumerable<ITriplesMap> TriplesMaps { get; }
    }
}