﻿#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Anotar.NLog;
using NullGuard;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Default implementation of the <a href="http://www.w3.org/TR/r2rml/#dfn-generated-rdf-term">RDF term generation process</a>
    /// </summary>
    public class RDFTermGenerator : IRDFTermGenerator
    {
        private static readonly Regex TemplateReplaceRegex = new Regex(@"(?<N>\{)([^\{\}.]+)(?<-N>\})(?(N)(?!))");
        private readonly IDictionary<string, IBlankNode> _blankNodeSubjects = new Dictionary<string, IBlankNode>(256);
        private readonly IDictionary<string, IBlankNode> _blankNodeObjects = new Dictionary<string, IBlankNode>(256);
        private INodeFactory _nodeFactory = new NodeFactory();
        private ISQLValuesMappingStrategy _sqlValuesMappingStrategy;

        public RDFTermGenerator(MappingOptions options)
        {
            Options = options;
            _sqlValuesMappingStrategy = new DefaultSQLValuesMappingStrategy(options);
        }

        /// <summary>
        /// Gets or sets the <see cref="ISQLValuesMappingStrategy"/>
        /// </summary>
        public ISQLValuesMappingStrategy SqlValuesMappingStrategy
        {
            get { return _sqlValuesMappingStrategy; }
            set { _sqlValuesMappingStrategy = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="INodeFactory"/>
        /// </summary>
        public INodeFactory NodeFactory
        {
            get { return _nodeFactory; }
            set { _nodeFactory = value; }
        }

        private MappingOptions Options { get; }

        /// <inheritdoc />
        [return: AllowNull]
        public TNodeType GenerateTerm<TNodeType>(ITermMap termMap, IDataRecord logicalRow)
            where TNodeType : class, INode
        {
            INode node = null;

            var logicalRowWrapped = new UndelimitedColumnsDataRecordWrapper(logicalRow);

            if (!(termMap.IsColumnValued || termMap.IsConstantValued || termMap.IsTemplateValued))
            {
                if (termMap is ISubjectMap && this.Options.AllowAutomaticBlankNodeSubjects)
                {
                    node = NodeFactory.CreateBlankNode();
                }
                else
                {
                    throw new InvalidTermException(termMap, "Term map must be either constant, column or template valued");
                }
            }

            if (termMap.IsConstantValued)
            {
                node = CreateNodeFromConstant(termMap);
            }
            else if (termMap.IsColumnValued)
            {
                node = CreateNodeFromColumn(termMap, logicalRowWrapped);
            }
            else if (termMap.IsTemplateValued)
            {
                node = CreateNodeFromTemplate(termMap, logicalRowWrapped);
            }

            if (node == null)
            {
                LogTo.Debug("Null term generated from term map {0}", termMap.Node);
            }
            else
            {
                LogTo.Debug("Generated term {0}", node);
            }

            return (TNodeType)node;
        }

        internal INode GenerateTermForValue(ITermMap termMap, string value)
        {
            if (value == null)
            {
                return null;
            }

            if (termMap.TermType.IsURI)
            {
                try
                {
                    var uri = new Uri(value, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        uri = ConstructAbsoluteUri(termMap, value);
                    }

                    return NodeFactory.CreateUriNode(uri);
                }
                catch (UriFormatException ex)
                {
                    throw new InvalidTermException(termMap, $"Value {value} is invalid. {ex.Message}");
                }
            }

            if (termMap.TermType.IsBlankNode)
            {
                return GenerateBlankNodeForValue(termMap, value);
            }

            if (termMap.TermType.IsLiteral)
            {
                var literalTermMap = termMap as ILiteralTermMap;

                return GenerateTermForLiteral(literalTermMap, value);
            }

            throw new InvalidTermException(termMap, "Term map must be either IRI-, literal- or blank node-valued");
        }

        /// <summary>
        /// Create an RDF term for a <a href="http://www.w3.org/TR/r2rml/#from-template">template-valued</a> <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
        /// </summary>
        protected INode CreateNodeFromTemplate(ITermMap termMap, IDataRecord logicalRow)
        {
            if (string.IsNullOrWhiteSpace(termMap.Template))
            {
                throw new InvalidTemplateException(termMap);
            }

            string value = null;
            try
            {
                value = ReplaceColumnReferences(termMap.Template, logicalRow, termMap.TermType.IsURI);
            }
            catch (IndexOutOfRangeException)
            {
            }

            if (value != null)
            {
                return GenerateTermForValue(termMap, value.Replace(@"\{", "{").Replace(@"\}", "}"));
            }

            return null;
        }

        /// <summary>
        /// Create an RDF term for a <a href="http://www.w3.org/TR/r2rml/#constant">constant-valued</a> <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
        /// </summary>
        protected INode CreateNodeFromConstant(ITermMap termMap)
        {
            var uriValuedTermMap = termMap as IUriValuedTermMap;
            if (uriValuedTermMap != null)
            {
                if (uriValuedTermMap.URI == null)
                {
                    throw new InvalidTermException(termMap, "IRI-valued term map must have IRI set");
                }

                return NodeFactory.CreateUriNode(uriValuedTermMap.URI);
            }

            var objectMap = termMap as IObjectMap;
            if (objectMap != null)
            {
                if (objectMap.URI != null && objectMap.Literal != null)
                {
                    throw new InvalidTermException(termMap, "Object map's value cannot be both IRI and literal.");
                }

                if (objectMap.URI != null)
                {
                    return NodeFactory.CreateUriNode(objectMap.URI);
                }

                if (objectMap.Literal != null)
                {
                    return NodeFactory.CreateLiteralNode(objectMap.Literal);
                }

                throw new InvalidTermException(termMap, "Neither IRI nor literal was set");
            }

            return null;
        }

        /// <summary>
        /// Create an RDF term for a <a href="http://www.w3.org/TR/r2rml/#from-column">column-valued</a> <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
        /// </summary>
        protected INode CreateNodeFromColumn(ITermMap termMap, IDataRecord logicalRow)
        {
            int columnIndex;
            try
            {
                columnIndex = logicalRow.GetOrdinal(termMap.ColumnName);
            }
            catch (IndexOutOfRangeException)
            {
                LogTo.Error("Column {0} was not found", termMap.ColumnName);
                throw new InvalidMapException(string.Format("Column {0} not found", termMap.ColumnName));
            }

            if (logicalRow.IsDBNull(columnIndex))
            {
                LogTo.Debug("Value was null in column {0}", termMap.ColumnName);
                return null;
            }

            Uri implicitDatatype;
            string value = SqlValuesMappingStrategy.GetLexicalForm(columnIndex, logicalRow, out implicitDatatype);

            if (termMap.TermType.IsLiteral)
            {
                return GenerateTermForLiteral(termMap, value, implicitDatatype);
            }
            
            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                return NodeFactory.CreateUriNode(new Uri(value)); 
            }

            AssertNoIllegalCharacters(termMap, new Uri(value, UriKind.RelativeOrAbsolute));

            return GenerateTermForValue(termMap, value);
        }

        /// <summary>
        /// Throws <see cref="InvalidTermException"/> is uri contains any . or .. segments
        /// </summary>
        private Uri ConstructAbsoluteUri(ITermMap termMap, string relativePart)
        {
            if (relativePart.Split('/').Any(seg => seg == "." || seg == ".."))
            {
                throw new InvalidTermException(termMap, "The relative IRI cannot contain any . or .. parts");
            }

            return new Uri(this.Options.BaseUri + relativePart);
        }

        private INode GenerateBlankNodeForValue(ITermMap termMap, string value)
        {
            IBlankNode blankNode;
            if (termMap is ISubjectMap)
            {
                if (!_blankNodeSubjects.ContainsKey(value))
                {
                    if (_blankNodeObjects.ContainsKey(value))
                    {
                        blankNode = _blankNodeObjects[value];
                    }
                    else
                    {
                        blankNode = _nodeFactory.CreateBlankNode();
                        _blankNodeObjects.Add(value, blankNode);
                    }

                    _blankNodeSubjects.Add(value, blankNode);
                }
                else if (this.Options.PreserveDuplicateRows)
                {
                    blankNode = _nodeFactory.CreateBlankNode();
                }
                else
                {
                    blankNode = _blankNodeSubjects[value];
                }

                if (!_blankNodeObjects.ContainsKey(value))
                {
                    _blankNodeObjects.Add(value, blankNode);
                }
            }
            else
            {
                if (!_blankNodeObjects.ContainsKey(value))
                {
                    _blankNodeObjects.Add(value, _nodeFactory.CreateBlankNode());
                }

                blankNode = _blankNodeObjects[value];
            }

            return blankNode;
        }

        private string ReplaceColumnReferences(string template, IDataRecord logicalRow, bool escape)
        {
            try
            {
                return TemplateReplaceRegex.Replace(
                    template,
                    match =>
                    {
                        var replacement = ReplaceColumnReference(match, logicalRow);
                        return escape ? MappingHelper.UrlEncode(replacement) : replacement;
                    });
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        private string ReplaceColumnReference(Match match, IDataRecord logicalRow)
        {
            var columnName = match.Captures[0].Value.TrimStart('{').TrimEnd('}');
            int columnIndex = logicalRow.GetOrdinal(columnName);

            if (logicalRow.IsDBNull(columnIndex))
            {
                LogTo.Debug("Value was null in column {0}", columnName);
                throw new ArgumentNullException();
            }

            Uri datatype;
            return SqlValuesMappingStrategy.GetLexicalForm(columnIndex, logicalRow, out datatype);
        }

        private ILiteralNode GenerateTermForLiteral(ITermMap termMap, string value, Uri datatypeUriOverride = null)
        {
            if (!(termMap is ILiteralTermMap))
            {
                throw new InvalidTermException(termMap, "Term map cannot be of term type literal");
            }

            var literalTermMap = termMap as ILiteralTermMap;
            Uri datatypeUri = literalTermMap.DataTypeURI ?? datatypeUriOverride;
            string language = literalTermMap.Language;

            if (language != null && datatypeUri != null)
            {
                throw new InvalidTermException(literalTermMap, "Literal term map cannot have both language tag and datatype set");
            }

            if (language != null)
            {
                return NodeFactory.CreateLiteralNode(value, language);
            }

            if (datatypeUri != null)
            {
                return NodeFactory.CreateLiteralNode(value, datatypeUri);
            }

            return NodeFactory.CreateLiteralNode(value);
        }

        private void AssertNoIllegalCharacters(ITermMap termMap, Uri value)
        {
            IEnumerable<char> disallowedChars = string.Empty;
            IEnumerable<string> segments = value.IsAbsoluteUri ? value.Segments : new[] { value.OriginalString };

            foreach (var segment in segments)
            {
                if (segment.Any(chara => !MappingHelper.IsIUnreserved(chara)))
                {
                    disallowedChars =
                        disallowedChars.Union(
                            segment.Where(chara => chara != '/' && !MappingHelper.IsIUnreserved(chara)));
                }
            }

            var joinedChars = string.Join(",", disallowedChars.Select(c => string.Format("'{0}'", c)));
            if (joinedChars.Any())
            {
                const string format = "Column value is not escaped and thus cannot contain these disallowed characters: {0}";
                var reason = string.Format(format, joinedChars);
                throw new InvalidTermException(termMap, reason);
            }
        }
    }
}