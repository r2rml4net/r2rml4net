using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface IRDFTermGenerator
    {
        TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow) where TNodeType : INode;
    }
}