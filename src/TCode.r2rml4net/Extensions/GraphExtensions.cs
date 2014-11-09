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
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Extensions
{
    public static class GraphExtensions
    {
        [return: AllowNull]
        public static INode GetSingleObject(this INode termMapNode, string predicate, [AllowNull] Func<IEnumerable<INode>, Exception> multipleObjectError = null)
        {
            var predicateNode = termMapNode.Graph.CreateUriNode(predicate);
            var triples = termMapNode.Graph.GetTriplesWithSubjectPredicate(termMapNode, predicateNode).ToArray();

            multipleObjectError = multipleObjectError ?? (nodes => MultipleResultsException(termMapNode, nodes, predicateNode));

            if (triples.Length > 1)
                throw multipleObjectError(triples.Select(GetObject));

            return triples.SingleOrDefault().GetObject();
        }

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

        private static INode GetObject([AllowNull] this Triple node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Object;
        }

        private static InvalidMapException MultipleResultsException(INode termMapNode, IEnumerable<INode> nodes, IUriNode predicateNode)
        {
            return new InvalidMapException(
                string.Format("Term map {1} contains multiple values for predicate {2}:\r\n{0}",
                              string.Join("\r\n", nodes.Select(node => node.ToString())),
                              termMapNode,
                              predicateNode.Uri));
        }
    }
}