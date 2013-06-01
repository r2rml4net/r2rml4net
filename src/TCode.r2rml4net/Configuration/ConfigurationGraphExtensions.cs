using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;

namespace TCode.r2rml4net.Configuration
{
    internal static class ConfigurationGraphExtensions
    {
        public static INode GetSingleTripleObject(this IGraph graph, INode objNode, Uri predicateUri)
        {
            return graph.GetTriples(objNode, predicateUri).Single().Object;
        }
        public static INode GetSingleOrDefaultTripleObject(this IGraph graph, INode objNode, Uri predicateUri)
        {
            var singleOrDefault = graph.GetTriples(objNode, predicateUri).SingleOrDefault();
            if (singleOrDefault != null)
            {
                return singleOrDefault.Object;
            }

            return null;
        }

        private static IEnumerable<Triple> GetTriples(this IGraph graph, INode objNode, Uri predicateUri)
        {
            var pred = graph.CreateUriNode(predicateUri);
            return graph.GetTriplesWithSubjectPredicate(objNode, pred);
        }
    }
}