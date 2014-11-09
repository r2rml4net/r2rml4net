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
using NullGuard;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Extensions;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Base fluent configuration of term maps (subject maps, predicate maps, graph maps or object maps) 
    /// backed by a DotNetRDF graph (see <see cref="ITermMapConfiguration"/>)
    /// </summary>
    public abstract class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ITermTypeConfiguration, ITermType
    {
        /// <summary>
        /// Creates a new instance of <see cref="TermMapConfiguration"/>
        /// </summary>
        protected TermMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IMapBase parentMap, IGraph r2RMLMappings, INode node) 
            : base(parentTriplesMap, r2RMLMappings, node)
        {
            ParentMapNode = parentMap.Node;
        }

        /// <inheritdoc/>
        public string ColumnName
        {
            [return: AllowNull]
            get { return Node.GetObjects(R2RMLUris.RrColumnProperty).GetSingleOrDefault().GetLiteral(); }
        }

        /// <inheritdoc/>
        public string Template
        {
            [return: AllowNull]
            get { return Node.GetObjects(R2RMLUris.RrTemplateProperty).GetSingleOrDefault().GetLiteral(); }
        }

        /// <inheritdoc/>
        public ITermTypeConfiguration TermType
        {
            get { return this; }
        }

        /// <inheritdoc/>
        public virtual Uri TermTypeURI
        {
            get
            {
                return ExplicitTermType ?? R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI).Uri;
            }
        }

        /// <inheritdoc/>
        public string InverseExpression
        {
            [return: AllowNull]
            get
            {
                return Node.GetObjects(R2RMLUris.RrInverseExpressionProperty)
                           .GetSingleOrDefault()
                           .GetLiteral(() => new InvalidMapException("An inverse expression must be a literal node"));
            }
        }

        bool ITermMap.IsConstantValued
        {
            get { return ConstantValue != null; }
        }

        bool ITermMap.IsColumnValued
        {
            get { return ColumnName != null; }
        }

        bool ITermMap.IsTemplateValued
        {
            get { return Template != null; }
        }

        ITermType ITermMap.TermType
        {
            get { return this; }
        }

        /// <inheritdoc />
        public bool IsURI
        {
            get { return R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI).Uri.AbsoluteUri.Equals(TermTypeURI.AbsoluteUri); }
        }

        /// <inheritdoc />
        bool ITermType.IsBlankNode
        {
            get { return R2RMLMappings.CreateUriNode(R2RMLUris.RrBlankNode).Uri.AbsoluteUri.Equals(TermTypeURI.AbsoluteUri); }
        }

        /// <inheritdoc />
        bool ITermType.IsLiteral
        {
            get { return R2RMLMappings.CreateUriNode(R2RMLUris.RrLiteral).Uri.AbsoluteUri.Equals(TermTypeURI.AbsoluteUri); }
        }

        /// <inheritdoc/>
        protected internal Uri ConstantValue
        {
            [return: AllowNull]
            get { return Node.GetObjects(R2RMLUris.RrConstantProperty).GetSingleOrDefault().GetUri(); }
        }

        /// <summary>
        /// Gets the parent node for the current term map
        /// </summary>
        /// <remarks>
        /// Depending on the type the parent can be a triples map (for property-object maps, subject maps), 
        /// property-object map (for object maps, property maps and graph maps)
        /// or subject map (for graph maps)
        /// </remarks>
        protected internal INode ParentMapNode { get; private set; }

        /// <summary>
        /// Gets explicitly set term type
        /// </summary>
        protected Uri ExplicitTermType
        {
            get
            {
                return Node.GetObjects(R2RMLUris.RrTermTypeProperty)
                           .GetSingleOrDefault(nodes => new InvalidMapException(string.Format("TermMap has {0} (should be zero or one)", nodes.Count())))
                           .GetUri(() => new InvalidMapException("Term type must be an IRI"));
            }
        }

        /// <summary>
        /// <see cref="INonLiteralTermMapConfigutarion.IsColumnValued"/>
        /// </summary>
        public void IsColumnValued(string columnName)
        {
            if (ColumnName != null)
            {
                throw new InvalidMapException("Term map can have only one column name");
            }

            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrColumnProperty), R2RMLMappings.CreateLiteralNode(columnName));
        }

        /// <inheritdoc/>
        public ITermTypeConfiguration IsConstantValued(Uri uri)
        {
            if (ConstantValue != null)
            {
                throw new InvalidMapException("Term map can have at most one constant value");
            }

            if (InverseExpression != null)
            {
                throw new InvalidMapException("Only column-valued term map or template-value term map can have an inverse expression");
            }

            EnsureRelationWithParentMap();

            R2RMLMappings.Retract(R2RMLMappings.GetTriplesWithSubjectPredicate(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrConstantProperty)));
            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrConstantProperty), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        /// <summary>
        /// <see cref="ITermMapConfiguration.IsTemplateValued(string)"/>
        /// </summary>
        public ITermTypeConfiguration IsTemplateValued(string template)
        {
            IUriNode templateProperty = R2RMLMappings.CreateUriNode(R2RMLUris.RrTemplateProperty);
            if (R2RMLMappings.GetTriplesWithSubjectPredicate(Node, templateProperty).Any())
            {
                throw new InvalidMapException("Term map can have at most one template");
            }

            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, templateProperty, R2RMLMappings.CreateLiteralNode(template));
            return this;
        }

        /// <summary>
        /// Sets the inverse expression. <see cref="ITermMapConfiguration.SetInverseExpression"/>
        /// </summary>
        public ITermMapConfiguration SetInverseExpression(string stringTemplate)
        {
            if (ConstantValue != null)
            {
                throw new InvalidMapException("An inverse expression can be only associated with a column-valued term map or template-value term map");
            }

            R2RMLMappings.Assert(
                Node,
                R2RMLMappings.CreateUriNode(R2RMLUris.RrInverseExpressionProperty),
                R2RMLMappings.CreateLiteralNode(stringTemplate));

            return this;
        }

        /// <inheritdoc/>
        public virtual ITermMapConfiguration IsBlankNode()
        {
            AssertTermTypeNotSet();
            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty), R2RMLMappings.CreateUriNode(R2RMLUris.RrBlankNode));
            return this;
        }

        /// <inheritdoc/>
        public virtual ITermMapConfiguration IsIRI()
        {
            AssertTermTypeNotSet();
            EnsureRelationWithParentMap();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrTermTypeProperty), R2RMLMappings.CreateUriNode(R2RMLUris.RrIRI));
            return this;
        }

        /// <inheritdoc/>
        public virtual ITermMapConfiguration IsLiteral()
        {
            throw new InvalidMapException("Only object map can be of term type rr:Literal");
        }

        /// <summary>
        /// Returns a term map property
        /// </summary>
        /// <returns>one of the following: rr:subjectMap, rr:objectMap, rr:propertyMap or rr:graphMap</returns>
        protected internal abstract IUriNode CreateMapPropertyNode();

        /// <summary>
        /// Returns a constant term shortcut property
        /// </summary>
        /// <returns>one of the following: rr:subject, rr:object, rr:property or rr:graph</returns>
        protected internal abstract IUriNode CreateShortcutPropertyNode();

        /// <summary>
        /// Checks wheather term type is already set
        /// </summary>
        protected void AssertTermTypeNotSet()
        {
            if (ExplicitTermType != null)
            {
                throw new InvalidMapException("Term type already set");
            }
        }

        /// <summary>
        /// Ensures that <see cref="ParentMapNode"/> and this <see cref="BaseConfiguration.Node"/> are 
        /// connected with the appropriate property
        /// </summary>
        /// <remarks>The two nodes will be connected by return value of <see cref="CreateMapPropertyNode"/> method</remarks>
        protected void EnsureRelationWithParentMap()
        {
            var containsMapPropertyTriple = R2RMLMappings.ContainsTriple(new Triple(ParentMapNode, CreateMapPropertyNode(), Node));

            if (!containsMapPropertyTriple)
            {
                CreateParentMapRelation();
            }
        }

        /// <summary>
        /// Creates relation with parent map (using rr:subjectMap, rr:objectMap, rr:propertyMap or rr:graphMap)
        /// </summary>
        protected void CreateParentMapRelation()
        {
            R2RMLMappings.Assert(ParentMapNode, CreateMapPropertyNode(), Node);
        }
    }
}
