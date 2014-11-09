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
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using TCode.r2rml4net.Exceptions;
using VDS.RDF;

namespace TCode.r2rml4net.Extensions
{
    /// <summary>
    /// Extension methods for accessing nodes' values
    /// </summary>
    public static class GraphExtensions
    {
        /// <summary>
        /// Gets the objects for given subject <paramref name="termMapNode"/> and <paramref name="predicate"/>.
        /// </summary>
        /// <param name="termMapNode">The subject node.</param>
        /// <param name="predicate">The predicate QName.</param>
        public static IEnumerable<INode> GetObjects(this INode termMapNode, string predicate)
        {
            var predicateNode = termMapNode.Graph.CreateUriNode(predicate);

            return termMapNode.Graph.GetTriplesWithSubjectPredicate(termMapNode, predicateNode).Select(triple => triple.Object);
        }

        /// <summary>
        /// Gets the single node or null.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="multipleObjectError">Exception to throw if there are multiple nodes.</param>
        [return: AllowNull]
        public static INode GetSingleOrDefault(this IEnumerable<INode> nodes, [AllowNull] Func<IEnumerable<INode>, Exception> multipleObjectError = null)
        {
            multipleObjectError = multipleObjectError ?? MultipleResultsException;

            var enumerable = nodes as INode[] ?? nodes.ToArray();
            if (enumerable.Length > 1)
            {
                throw multipleObjectError(enumerable);
            }

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// Gets the literal value of a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="getError">[Optional] Exception to throw if <paramref name="node"/> is not literal.</param>
        /// <returns>literal value or null is <paramref name="node"/> is not literal</returns>
        [return: AllowNull]
        public static string GetLiteral([AllowNull] this INode node, [AllowNull] Func<Exception> getError = null)
        {
            var literalNode = node as ILiteralNode;

            if (literalNode != null)
            {
                return literalNode.Value;
            }

            if (node != null && getError != null)
            {
                throw getError();
            }

            return null;
        }

        /// <summary>
        /// Gets the URI value of a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="getError">[Optional] Exception to throw if <paramref name="node"/> is not literal.</param>
        /// <returns>URI value or null is <paramref name="node"/> is not literal</returns>
        [return: AllowNull]
        public static Uri GetUri([AllowNull] this INode node, [AllowNull] Func<Exception> getError = null)
        {
            var uriNode = node as IUriNode;

            if (uriNode != null)
            {
                return uriNode.Uri;
            }

            if (node != null && getError != null)
            {
                throw getError();
            }

            return null;
        }

        /// <summary>
        /// Gets the datatype URI of a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="getError">[Optional] Exception to throw if <paramref name="node"/> is not literal.</param>
        /// <returns>literal datatype or null is <paramref name="node"/> is not literal</returns>
        [return: AllowNull]
        public static Uri GetDatatype([AllowNull] this INode node, [AllowNull] Func<Exception> getError = null)
        {
            var uriNode = node as ILiteralNode;

            if (uriNode != null)
            {
                return uriNode.DataType;
            }

            if (node != null && getError != null)
            {
                throw getError();
            }

            return null;
        }

        /// <summary>
        /// Gets the language value of a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="getError">[Optional] Exception to throw if <paramref name="node"/> is not literal.</param>
        /// <returns>literal language or null is <paramref name="node"/> is not literal</returns>
        [return: AllowNull]
        public static string GetLanguageTag([AllowNull] this INode node, [AllowNull] Func<Exception> getError = null)
        {
            var uriNode = node as ILiteralNode;

            if (uriNode != null)
            {
                return uriNode.Language;
            }

            if (node != null && getError != null)
            {
                throw getError();
            }

            return null;
        }

        private static InvalidMapException MultipleResultsException(IEnumerable<INode> nodes)
        {
            return new InvalidMapException(
                string.Format("Expected at most on values for predicate but got:\r\n{0}",
                              string.Join("\r\n", nodes.Select(node => node.ToString()))));
        }
    }
}