using System;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests
{
    static class GraphTestsExtensions
    {
        #region VerifyHasTriple

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, string subjectUri, string predicateUri, string objectUri)
        {
            graph.VerifyHasTriple(new Uri(subjectUri), new Uri(predicateUri), new Uri(objectUri));
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, Uri subjectUri, string predicateUri, string objectUri)
        {
            graph.VerifyHasTriple(subjectUri, new Uri(predicateUri), new Uri(objectUri));
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, string subjectUri, Uri predicateUri, string objectUri)
        {
            graph.VerifyHasTriple(new Uri(subjectUri), predicateUri, new Uri(objectUri));
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, string subjectUri, string predicateUri, Uri objectUri)
        {
            graph.VerifyHasTriple(new Uri(subjectUri), new Uri(predicateUri), objectUri);
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, Uri subjectUri, Uri predicateUri, string objectUri)
        {
            graph.VerifyHasTriple(subjectUri, predicateUri, new Uri(objectUri));
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, Uri subjectUri, string predicateUri, Uri objectUri)
        {
            graph.VerifyHasTriple(subjectUri, new Uri(predicateUri), objectUri);
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, string subjectUri, Uri predicateUri, Uri objectUri)
        {
            graph.VerifyHasTriple(new Uri(subjectUri), predicateUri, objectUri);
        }

        /// <summary>
        /// Check wheather graph <paramref name="graph"/> has a triple
        /// </summary>
        internal static void VerifyHasTriple(this IGraph graph, Uri subjectUri, Uri predicateUri, Uri objectUri)
        {
            Assert.IsTrue(graph.ContainsTriple(new Triple(
                graph.CreateUriNode(subjectUri),
                graph.CreateUriNode(predicateUri),
                graph.CreateUriNode(objectUri)
                )), string.Format("Triple <{0}> => <{1}> => <{2}> not found in graph", subjectUri, predicateUri, objectUri));
        } 

        #endregion


    }
}