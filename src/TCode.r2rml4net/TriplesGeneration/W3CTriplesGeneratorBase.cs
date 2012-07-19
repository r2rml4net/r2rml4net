using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Base implementation of the triple generation algorithm suggested by R2RML specification. 
    /// It should generate triples for all rows in all triples maps from the input R2RML mappings
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#generated-rdf</remarks>
    public abstract class W3CTriplesGeneratorBase : ITriplesGenerator
    {
        #region Implementation of ITriplesGenerator

        public ITripleStore GenerateTriples(IR2RML r2RML)
        {
            foreach (var triplesMap in r2RML.TriplesMaps)
            {
                ProcessTriplesMap(triplesMap);
            }

            return null;
        }

        #endregion

        protected internal virtual void ProcessTriplesMap(ITriplesMap triplesMap)
        {
        }
    }
}