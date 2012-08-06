using System;
using VDS.RDF;

namespace TCode.r2rml4net.RDF
{
    public static class TripleExtensions
    {
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