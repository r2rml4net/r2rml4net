using System.Collections.Generic;

namespace TCode.r2rml4net.RDF
{
    public interface IR2RML
    {
        IEnumerable<ITriplesMap> TriplesMaps { get; }
    }
}