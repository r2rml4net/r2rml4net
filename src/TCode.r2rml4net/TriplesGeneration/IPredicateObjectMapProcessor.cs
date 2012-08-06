using System.Collections.Generic;
using System.Data;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Interface for classes which process <see cref="IPredicateObjectMap"/> in order to generate RDF triples
    /// </summary>
    public interface IPredicateObjectMapProcessor
    {
        /// <summary>
        /// Processes the <paramref name="predicateObjectMap"/> and asserts triples for the cartesian of terms generated for
        /// <paramref name="subject"/>, <see cref="IPredicateObjectMap.PredicateMaps"/>, <see cref="IPredicateObjectMap.ObjectMaps"/>
        /// and a union of <see cref="IPredicateObjectMap.GraphMaps"/> and <paramref name="subjectGraphs"/>
        /// </summary>
        void ProcessPredicateObjectMap(INode subject, IPredicateObjectMap predicateObjectMap, IEnumerable<IUriNode> subjectGraphs, IDataRecord logicalRow, IRdfHandler rdfHandler);
    }
}