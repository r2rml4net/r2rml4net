#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Direct
{
    /// <summary>
    /// Default implementation of <see cref="IDirectMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class DirectMappingStrategy : MappingStrategyBase, IDirectMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;
        private IForeignKeyMappingStrategy _foreignKeyMappingStrategy;

        /// <summary>
        /// Creates a new instance of <see cref="DirectMappingStrategy"/> with default options
        /// </summary>
        public DirectMappingStrategy()
            : this(new MappingOptions())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectMappingStrategy"/> with custom options
        /// </summary>
        public DirectMappingStrategy(MappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IDirectMappingStrategy

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> as a template valued blank node with template 
        /// returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectTemplateForNoPrimaryKey"/>
        /// and class returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectClassUri"/>
        /// </summary>
        public virtual void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri BaseUri, TableMetadata table)
        {
            if (subjectMap == null)
                throw new ArgumentNullException("subjectMap");
            if (BaseUri == null)
                throw new ArgumentNullException("BaseUri");
            if (table == null)
                throw new ArgumentNullException("table");

            if (table.PrimaryKey.Length != 0)
                throw new ArgumentException(string.Format("Table {0} has primay key. CreateSubjectMapForPrimaryKey method should be used", table.Name));

            string template = PrimaryKeyMappingStrategy.CreateSubjectTemplateForNoPrimaryKey(table);
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(BaseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> as a termplate valued URI noed with template 
        /// returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectTemplateForPrimaryKey"/>
        /// and class returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectClassUri"/>
        /// </summary>
        public virtual void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri BaseUri, TableMetadata table)
        {
            if (subjectMap == null)
                throw new ArgumentNullException("subjectMap");
            if (BaseUri == null)
                throw new ArgumentNullException("BaseUri");
            if (table == null)
                throw new ArgumentNullException("table");

            if(table.PrimaryKey.Length == 0)
                throw new ArgumentException(string.Format("Table {0} has no primay key", table.Name));

            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(BaseUri, table.Name);

            string template = PrimaryKeyMappingStrategy.CreateSubjectTemplateForPrimaryKey(BaseUri, table);

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#dfn-predicate-map">predicate map</a> as constant valued with URI returned by
        /// <see cref="IForeignKeyMappingStrategy.CreateReferencePredicateUri"/>
        /// </summary>
        public virtual void CreatePredicateMapForForeignKey(ITermMapConfiguration predicateMap, Uri BaseUri, ForeignKeyMetadata foreignKey)
        {
            Uri foreignKeyRefUri = ForeignKeyMappingStrategy.CreateReferencePredicateUri(BaseUri, foreignKey);
            predicateMap.IsConstantValued(foreignKeyRefUri);
        }

        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> as template valued blank node with template
        /// returned by <see cref="IForeignKeyMappingStrategy.CreateObjectTemplateForCandidateKeyReference"/>
        /// </summary>
        public virtual void CreateObjectMapForCandidateKeyReference(IObjectMapConfiguration objectMap, ForeignKeyMetadata foreignKey)
        {
            objectMap
                .IsTemplateValued(ForeignKeyMappingStrategy.CreateObjectTemplateForCandidateKeyReference(foreignKey))
                .IsBlankNode();
        }

        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> as template valued node with template returned by
        /// <see cref="IForeignKeyMappingStrategy.CreateReferenceObjectTemplate"/>
        /// </summary>
        public virtual void CreateObjectMapForPrimaryKeyReference(IObjectMapConfiguration objectMap, Uri BaseUri, ForeignKeyMetadata foreignKey)
        {
            var templateForForeignKey = ForeignKeyMappingStrategy.CreateReferenceObjectTemplate(BaseUri, foreignKey);
            objectMap.IsTemplateValued(templateForForeignKey);
        }

        #endregion

        /// <summary>
        /// Strategy for mapping primary keys
        /// </summary>
        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy(Options);

                return _primaryKeyMappingStrategy;
            }
            set { _primaryKeyMappingStrategy = value; }
        }

        /// <summary>
        /// Strategy for mapping foreign keys
        /// </summary>
        public IForeignKeyMappingStrategy ForeignKeyMappingStrategy
        {
            get
            {
                if (_foreignKeyMappingStrategy == null)
                    _foreignKeyMappingStrategy = new ForeignKeyMappingStrategy(Options);

                return _foreignKeyMappingStrategy;
            }
            set { _foreignKeyMappingStrategy = value; }
        }
    }
}