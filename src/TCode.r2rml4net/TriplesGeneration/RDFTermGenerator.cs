using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    class RDFTermGenerator : IRDFTermGenerator
    {
        #region Implementation of IRDFTermGenerator

        public INode GenerateTerm(ITermMap termMap, IDataRecord logicalRow)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}