using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    class W3CPredicateObjectMapProcessor : IPredicateObjectMapProcessor
    {
        private readonly IRDFTermGenerator _termGenerator;
        private readonly IRdfHandler _rdfHandler;

        public W3CPredicateObjectMapProcessor(IRDFTermGenerator termGenerator, IRdfHandler rdfHandler)
        {
            _termGenerator = termGenerator;
            _rdfHandler = rdfHandler;
        }

        #region Implementation of IPredicateObjectMapProcessor

        public void ProcessPredicateObjectMap(IPredicateObjectMap predicateObjectMap, IDataRecord logicalRow)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}