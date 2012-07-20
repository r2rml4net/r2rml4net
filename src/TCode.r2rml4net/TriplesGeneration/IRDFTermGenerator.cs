using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface IRDFTermGenerator
    {
        INode GenerateTerm(ITermMap termMap, IDataRecord logicalRow);
    }
}