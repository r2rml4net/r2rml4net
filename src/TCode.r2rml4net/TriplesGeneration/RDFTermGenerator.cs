using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    class RDFTermGenerator : IRDFTermGenerator
    {
        #region Implementation of IRDFTermGenerator

        public TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow) where TNodeType : INode
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}