using System;
using VDS.RDF;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Extension methods for <see cref="Triple"/> class
    /// </summary>
    public static class TripleExtensions
    {
        /// <summary>
        /// Clone a triple and optionally replace <see cref="Triple.Subject"/>, <see cref="Triple.Predicate"/>, 
        /// <see cref="Triple.Object"/> or <see cref="Triple.Graph"/>
        /// </summary>
        public static Triple CloneTriple(
            this Triple t, 
            INode replacedSubject = null, 
            INode replacedPredicate = null, 
            INode replacedObject = null, 
            IGraph replacedGraph = null,
            Uri replacedGraphUri = null)
        {
            Triple newTriple;
            INode newSubject = replacedSubject ?? t.Subject;
            INode newPredicate = replacedPredicate ?? t.Predicate;
            INode newObject = replacedObject ?? t.Object;
            IGraph newGraph = replacedGraph ?? t.Graph;
            Uri newGraphUri = replacedGraphUri ?? t.GraphUri;

            if (newGraph == null && newGraphUri == null)
            {
                newTriple = new Triple(newSubject, newPredicate, newObject);
            }
            else if(newGraph != null)
            {
                if (replacedGraphUri != null)
                {
                    var message = string.Format("Tried to repalce by setting both newGraph and newGraphUri. Actual values are {0} and {1} respectively", newGraph, newGraphUri);
                    throw new ArgumentException(message, "replacedGraphUri");
                }

                newTriple = new Triple(newSubject, newPredicate, newObject, newGraph);
            }
            else
            {
                newTriple = new Triple(newSubject, newPredicate, newObject, newGraphUri);
            }

            return newTriple;
        }
    }
}