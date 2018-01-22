#region Licence
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
using System.Globalization;
using System.Linq;
using NullGuard;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Extensions;
using TCode.r2rml4net.RDF;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent
{
    [NullGuard(ValidationFlags.All)]
    internal class ObjectMapConfiguration : TermMapConfiguration, IObjectMapConfiguration, ILiteralTermMapConfiguration, ITermType
    {
        internal ObjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IPredicateObjectMapConfiguration parentMap, IGraph r2RMLMappings)
            : this(parentTriplesMap, parentMap, r2RMLMappings, r2RMLMappings.CreateBlankNode())
        {
        }

        internal ObjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IPredicateObjectMapConfiguration parentMap, IGraph r2RMLMappings, INode node)
            : base(parentTriplesMap, parentMap, r2RMLMappings, node)
        {
            LanguageTagValidator = new Bcp47RegexLanguageTagValidator();
        }

        bool ITermMap.IsConstantValued
        {
            get
            {
                return ConstantValue != null || !string.IsNullOrEmpty(Literal);
            }
        }

        public ILanguageTagValidator LanguageTagValidator { get; set; }

        public Uri URI
        {
            [return: AllowNull]
            get { return ConstantValue; }
        }

        public string Literal
        {
            [return: AllowNull]
            get { return Node.GetObjects(R2RMLUris.RrConstantProperty).GetSingleOrDefault().GetLiteral(); }
        }

        public Uri DataTypeURI
        {
            [return: AllowNull]
            get
            {
                var datatypeObject = Node.GetObjects(R2RMLUris.RrDatatypePropety).GetSingleOrDefault();
                if (datatypeObject != null)
                {
                    return GetRrDatatypeUri(datatypeObject);
                }

                return Node.GetObjects(R2RMLUris.RrConstantProperty)
                           .GetSingleOrDefault()
                           .GetDatatype();
            }
        }

        public string Language
        {
            [return: AllowNull]
            get
            {
                string languageTag;

                var languageObject = Node.GetObjects(R2RMLUris.RrLanguagePropety).GetSingleOrDefault();
                if (languageObject != null)
                {
                    languageTag = GetLanguageFromNode(languageObject);
                }
                else
                {
                    languageTag = Node.GetObjects(R2RMLUris.RrConstantProperty)
                                      .GetSingleOrDefault()
                                      .GetLanguageTag();
                }

                if (!string.IsNullOrWhiteSpace(languageTag))
                {
                    if (LanguageTagValidator.LanguageTagIsValid(languageTag))
                    {
                        return languageTag;
                    }

                    throw new InvalidTermException(this, string.Format("Language tag '{0}' is invalid", languageTag));
                }

                return null;
            }
        }

        /// <summary>
        /// Overriden, because object maps can be of term type rr:Literal
        /// </summary>
        public override Uri TermTypeURI
        {
            get
            {
                if (ExplicitTermType != null)
                {
                    return ExplicitTermType;
                }

                // term type is literal is column valued, or has datatype or language tag
                if (IsLiteralTermType)
                {
                    return R2RMLMappings.CreateUriNode(R2RMLUris.RrLiteral).Uri;
                }

                // in other cases is rr:IRI
                return R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI).Uri;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the term is of term type literal.
        /// </summary>
        /// <exception cref="TCode.r2rml4net.Exceptions.InvalidMapException">Object map cannot have both a rr:language and rr:datatype properties set</exception>
        /// <remarks>
        /// See http://www.w3.org/TR/r2rml/#termtype
        /// </remarks>
        private bool IsLiteralTermType
        {
            get
            {
                var columnObjects = Node.GetObjects(R2RMLUris.RrColumnProperty);
                if (columnObjects.Any())
                {
                    return true;
                }

                var hasLanguages = Node.GetObjects(R2RMLUris.RrLanguagePropety).Any();
                var hasDatatypes = Node.GetObjects(R2RMLUris.RrDatatypePropety).Any();

                if (hasLanguages && hasDatatypes)
                {
                    throw new InvalidMapException("Object map cannot have both a rr:language and rr:datatype properties set");
                }

                return hasLanguages || hasDatatypes;
            }
        }

        public ILiteralTermMapConfiguration IsConstantValued(string literal)
        {
            if (Literal != null)
            {
                throw new InvalidMapException("Term map can have at most one constant value");
            }

            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrConstantProperty), R2RMLMappings.CreateLiteralNode(literal));

            return this;
        }

        ILiteralTermMapConfiguration IObjectMapConfiguration.IsColumnValued(string columnName)
        {
            IsColumnValued(columnName);
            return this;
        }

        public override ITermMapConfiguration IsLiteral()
        {
            AssertTermTypeNotSet();
            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty), R2RMLMappings.CreateUriNode(R2RMLUris.RrLiteral));
            return this;
        }

        public void HasDataType(string dataTypeUri)
        {
            HasDataType(new Uri(dataTypeUri));
        }

        public void HasDataType(Uri dataTypeUri)
        {
            EnsureOnlyLanguageOrDatatype();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrDatatypePropety), R2RMLMappings.CreateUriNode(dataTypeUri));
        }

        public void HasLanguage(string languageTag)
        {
            EnsureOnlyLanguageOrDatatype();
            if (!LanguageTagValidator.LanguageTagIsValid(languageTag))
            {
                throw new ArgumentException(string.Format("Language tag '{0}' is invalid", languageTag), languageTag);
            }

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrLanguagePropety), R2RMLMappings.CreateLiteralNode(languageTag.ToLower()));
        }

        public void HasLanguage(CultureInfo cultureInfo)
        {
            HasLanguage(cultureInfo.Name);
        }

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty);
        }

        protected internal override IUriNode CreateShortcutPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectProperty);
        }

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            // object map contains no submaps
        }

        private void EnsureOnlyLanguageOrDatatype()
        {
            var datatypeNodes = Node.GetObjects(R2RMLUris.RrDatatypePropety);
            var languageNodes = Node.GetObjects(R2RMLUris.RrLanguagePropety);

            if (datatypeNodes.Any())
            {
                throw new InvalidMapException("Object map already has a datatype");
            }

            if (languageNodes.Any())
            {
                throw new InvalidMapException("Object map already has a language tag");
            }
        }

        private string GetLanguageFromNode(INode languageObject)
        {
            var datatypeTriple = Node.GetObjects(R2RMLUris.RrDatatypePropety);

            if (datatypeTriple.Any())
            {
                throw new InvalidTermException(this, "Object map has both language tag and datatype set");
            }

            return languageObject.GetLiteral(() => new InvalidMapException("Object map has language set but it is not a literal"));
        }

        private Uri GetRrDatatypeUri(INode datatypeObject)
        {
            var languageTriples = Node.GetObjects(R2RMLUris.RrLanguagePropety);

            if (languageTriples.Any())
            {
                throw new InvalidMapException("Object map has both language tag and datatype set");
            }

            return datatypeObject.GetUri(() => new InvalidMapException("Object map has datatype set but it is not a URI"));
        }
    }
}