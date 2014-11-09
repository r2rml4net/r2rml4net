#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using System.Linq;
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

        #region VerifyHasTripleWithBlankObject

        /// <summary>
        /// Check wheather graph has triples with subject and predicate and blank node object
        /// </summary>
        internal static void VerifyHasTripleWithBlankObject(this IGraph graph, string subjectUri, string predicateUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankObject(new Uri(subjectUri), new Uri(predicateUri), expectedTriplesCount);
        }

        /// <summary>
        /// Check wheather graph has triples with subject and predicate and blank node object
        /// </summary>
        internal static void VerifyHasTripleWithBlankObject(this IGraph graph, Uri subjectUri, string predicateUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankObject(subjectUri, new Uri(predicateUri), expectedTriplesCount);
        }

        /// <summary>
        /// Check wheather graph has triples with subject and predicate and blank node object
        /// </summary>
        internal static void VerifyHasTripleWithBlankObject(this IGraph graph, string subjectUri, Uri predicateUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankObject(new Uri(subjectUri), predicateUri, expectedTriplesCount);
        }

        /// <summary>
        /// Check wheather graph has triples with subject and predicate and blank node object
        /// </summary>
        internal static void VerifyHasTripleWithBlankObject(this IGraph graph, Uri subjectUri, Uri predicateUri, int expectedTriplesCount = 1)
        {
            var triples = graph.GetTriplesWithSubjectPredicate(
                graph.CreateUriNode(subjectUri),
                graph.CreateUriNode(predicateUri)
                ).ToArray();

            Assert.AreEqual(expectedTriplesCount, triples.Count());
            foreach (var triple in triples)
            {
                Assert.AreEqual(NodeType.Blank, triple.Object.NodeType, "Triple found but object was {0}", triple.Object.NodeType);
            }
        }

        #endregion

        #region VerifyHasTripleWithBlankSubjectAndLiteralObject

        /// <summary>
        /// Check wheather graph has a triple with blank subject, given predicate and literal object
        /// </summary>
        internal static void VerifyHasTripleWithBlankSubjectAndLiteralObject(this IGraph graph, string predicateUri, string literalValue, string languageSpec = null, Uri dataType = null, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankSubjectAndLiteralObject(
                new Uri(predicateUri),
                literalValue,
                languageSpec,
                dataType,
                expectedTriplesCount);
        }

        /// <summary>
        /// Check wheather graph has a triple with blank subject, given predicate and literal object
        /// </summary>
        internal static void VerifyHasTripleWithBlankSubjectAndLiteralObject(this IGraph graph, Uri predicateUri, string literalValue, string languageSpec = null, Uri dataType = null, int expectedTriplesCount = 1)
        {
            var literalNode = CreateLiteralNode(graph, literalValue, languageSpec, dataType);

            var triples = graph.GetTriplesWithPredicateObject(
                graph.CreateUriNode(predicateUri),
                literalNode
                ).ToArray();

            Assert.AreEqual(expectedTriplesCount, triples.Count());
            foreach (var triple in triples)
            {
                Assert.AreEqual(NodeType.Blank, triple.Subject.NodeType, "Triple found but subject was {0}", triple.Object.NodeType);
            }
        }

        #endregion

        #region VerifyHasTripleWithBlankSubject

        /// <summary>
        /// Checks wheather graph has triple with blank object
        /// </summary>
        public static void VerifyHasTripleWithBlankSubject(this IGraph graph, string predicateUri, string objectUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankSubject(new Uri(predicateUri), new Uri(objectUri));
        }

        /// <summary>
        /// Checks wheather graph has triple with blank object
        /// </summary>
        public static void VerifyHasTripleWithBlankSubject(this IGraph graph, Uri predicateUri, string objectUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankSubject(predicateUri, new Uri(objectUri));
        }

        /// <summary>
        /// Checks wheather graph has triple with blank object
        /// </summary>
        public static void VerifyHasTripleWithBlankSubject(this IGraph graph, string predicateUri, Uri objectUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankSubject(new Uri(predicateUri), objectUri);
        }

        /// <summary>
        /// Checks wheather graph has triple with blank object
        /// </summary>
        public static void VerifyHasTripleWithBlankSubject(this IGraph graph, Uri predicateUri, Uri objectUri, int expectedTriplesCount = 1)
        {
            var triples = graph.GetTriplesWithPredicateObject(
                graph.CreateUriNode(predicateUri),
                graph.CreateUriNode(objectUri)
                ).ToArray();

            Assert.AreEqual(expectedTriplesCount, triples.Count());
            foreach (var triple in triples)
            {
                Assert.AreEqual(NodeType.Blank, triple.Subject.NodeType, "Triple found but subject was {0}", triple.Subject.NodeType);
            }
        }

        #endregion

        #region VerifyHasTripleWithBlankSubjectAndObject

        public static void VerifyHasTripleWithBlankSubjectAndObject(this IGraph graph, string predicateUri, int expectedTriplesCount = 1)
        {
            graph.VerifyHasTripleWithBlankSubjectAndObject(new Uri(predicateUri), expectedTriplesCount);
        }

        public static void VerifyHasTripleWithBlankSubjectAndObject(this IGraph graph, Uri predicateUri, int expectedTriplesCount = 1)
        {
            var triples = graph.GetTriplesWithPredicate(graph.CreateUriNode(predicateUri)).ToArray();

            Assert.AreEqual(expectedTriplesCount, triples.Count());
            foreach (var triple in triples)
            {
                Assert.AreEqual(NodeType.Blank, triple.Subject.NodeType, "Triple found but subject was {0}", triple.Subject.NodeType);
                Assert.AreEqual(NodeType.Blank, triple.Object.NodeType, "Triple found but object was {0}", triple.Object.NodeType);
            }
        }

        #endregion

        private static ILiteralNode CreateLiteralNode(IGraph graph, string literalValue, string languageSpec, Uri dataType)
        {
            if (languageSpec != null && dataType != null)
                throw new ArgumentException("Both languageSpec and dataType params cannot be set");

            ILiteralNode literalNode;
            if (languageSpec == null && dataType == null)
                literalNode = graph.CreateLiteralNode(literalValue);
            else if (languageSpec != null)
                literalNode = graph.CreateLiteralNode(literalValue, languageSpec);
            else
                literalNode = graph.CreateLiteralNode(literalValue, dataType);
            return literalNode;
        }
    }
}