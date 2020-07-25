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
using System.Linq;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.Direct
{
    /// <summary>
    /// Builds a R2RML graph from a relational database's schema
    /// </summary>
    internal class R2RMLMappingGenerator : IDatabaseMetadataVisitor
    {
        private readonly IDatabaseMetadata _databaseMetadataProvider;
        private readonly IR2RMLConfiguration _r2RMLConfiguration;
        private ITriplesMapConfiguration _currentTriplesMapConfiguration;
        private IDirectMappingStrategy _mappingStrategy;
        private IColumnMappingStrategy _columnMappingStrategy;
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;

        /// <summary>
        /// Initializes a new instance of <see cref="R2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public R2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
        {
            _databaseMetadataProvider = databaseMetadataProvider;
            _r2RMLConfiguration = r2RMLConfiguration;

            MappingBaseUri = r2RMLConfiguration.MappingsGraph?.BaseUri;
        }

        /// <summary>
        /// Gets or sets the R2RML graph's base URI
        /// </summary>
        public Uri MappingBaseUri { get; set; }

        /// <summary>
        /// Gets or sets an implementation of <see cref="IDirectMappingStrategy"/>,
        /// which defines how to map relational database to RDF subject, predicate and object maps
        /// </summary>
        public IDirectMappingStrategy MappingStrategy
        {
            get
            {
                if (_mappingStrategy == null)
                {
                    _mappingStrategy = new DirectMappingStrategy(this._r2RMLConfiguration.Options);
                }

                return _mappingStrategy;
            }

            set
            {
                _mappingStrategy = value;
            }
        }

        /// <summary>
        /// Gets or sets an impementation of <see cref="IColumnMappingStrategy"/>,
        /// which defines how to map columns to RDF predicates
        /// </summary>
        public IColumnMappingStrategy ColumnMappingStrategy
        {
            get
            {
                if (_columnMappingStrategy == null)
                {
                    _columnMappingStrategy = new ColumnMappingStrategy();
                }

                return _columnMappingStrategy;
            }

            set
            {
                _columnMappingStrategy = value;
            }
        }

        /// <summary>
        /// Gets or sets  the implementation of <see cref="IPrimaryKeyMappingStrategy"/>,
        /// which defines how to map primary keys to RDF subjects
        /// </summary>
        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                {
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy(this._r2RMLConfiguration.Options);
                }

                return _primaryKeyMappingStrategy;
            }

            set
            {
                _primaryKeyMappingStrategy = value;
            }
        }

        internal ITriplesMapConfiguration CurrentTriplesMapConfiguration
        {
            get { return _currentTriplesMapConfiguration; }
            set { _currentTriplesMapConfiguration = value; }
        }

        /// <summary>
        /// Generates default R2RML mappings based on database metadata
        /// </summary>
        public IR2RML GenerateMappings()
        {
            if (_databaseMetadataProvider.Tables != null)
            {
                _databaseMetadataProvider.Tables.Accept(this);
            }

            return _r2RMLConfiguration;
        }

        #region Implementation of IDatabaseMetadataVisitor

        /// <summary>
        /// Visits a <see cref="TableCollection"/> and it's tables
        /// </summary>
        public void Visit(TableCollection tables)
        {
        }

        /// <summary>
        /// Visits a <see cref="TableMetadata"/> and it's columns
        /// </summary>
        public void Visit(TableMetadata table)
        {
            if (table.ForeignKeys.Any(fk => fk.IsCandidateKeyReference && fk.ReferencedTableHasPrimaryKey))
            {
                var rmlView = _r2RMLConfiguration.SqlQueryBuilder.GetR2RMLViewForJoinedTables(table);
                _currentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromR2RMLView(rmlView);
            }
            else
            {
                _currentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromTable(table.Name);
            }

            if (table.PrimaryKey.Length == 0)
            {
                MappingStrategy.CreateSubjectMapForNoPrimaryKey(CurrentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
            else
            {
                MappingStrategy.CreateSubjectMapForPrimaryKey(CurrentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
        }

        /// <summary>
        /// Visits a <see cref="ColumnMetadata"/>
        /// </summary>
        public void Visit(ColumnMetadata column)
        {
            Uri predicateUri = ColumnMappingStrategy.CreatePredicateUri(MappingBaseUri, column);

            var propertyObjectMap = CurrentTriplesMapConfiguration.CreatePropertyObjectMap();
            propertyObjectMap.CreatePredicateMap().IsConstantValued(predicateUri);
            var literalTermMap = propertyObjectMap.CreateObjectMap().IsColumnValued(column.Name);

            var dataTypeUri = XsdDatatypes.GetDataType(column.Type);
            if (dataTypeUri != null)
            {
                literalTermMap.HasDataType(dataTypeUri);
            }
        }

        /// <summary>
        /// Visist a <see cref="ForeignKeyMetadata"/>
        /// </summary>
        public void Visit(ForeignKeyMetadata foreignKey)
        {
            var foreignKeyMap = CurrentTriplesMapConfiguration.CreatePropertyObjectMap();

            MappingStrategy.CreatePredicateMapForForeignKey(foreignKeyMap.CreatePredicateMap(), MappingBaseUri, foreignKey);

            if (foreignKey.IsCandidateKeyReference && !foreignKey.ReferencedTableHasPrimaryKey)
            {
                MappingStrategy.CreateObjectMapForCandidateKeyReference(foreignKeyMap.CreateObjectMap(), foreignKey);
            }
            else
            {
                MappingStrategy.CreateObjectMapForPrimaryKeyReference(foreignKeyMap.CreateObjectMap(), MappingBaseUri, foreignKey);
            }
        }

        #endregion
    }
}
