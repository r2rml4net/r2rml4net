using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for generating RDF terms (<see cref="INode"/>s) for term maps
    /// </summary>
    /// <remarks>see http://www.w3.org/TR/r2rml/#dfn-generated-rdf-term</remarks>
    public interface IRDFTermGenerator
    {
        /// <summary>
        /// Generates RDF term for the given <see cref="ITermMap"/> by applying to the <paramref name="logicalRow"/>
        /// </summary>
        /// <remarks>see http://www.w3.org/TR/r2rml/#dfn-generated-rdf-term</remarks>
        /// <returns>an RDF term (<see cref="INode"/>)</returns>
        TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow) where TNodeType : INode;
    }
}