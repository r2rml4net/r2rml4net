using System;
using System.Linq;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Builds a R2RML graph from a relational database's schema
    /// </summary>
    public class R2RMLMappingGenerator : IDatabaseMetadataVisitor
    {
        private readonly IDatabaseMetadata _databaseMetadataProvider;
        private readonly IR2RMLConfiguration _r2RMLConfiguration;
        internal ITriplesMapConfiguration CurrentTriplesMapConfiguration;
        private IDirectMappingStrategy _mappingStrategy;
        private IColumnMappingStrategy _columnMappingStrategy;
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;
        private readonly MappingOptions _options;

        /// <summary>
        /// Creates <see cref="R2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public R2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration, MappingOptions options)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._r2RMLConfiguration = r2RMLConfiguration;
            _options = options;

            MappingBaseUri = r2RMLConfiguration.BaseUri;
            SqlBuilder = new W3CSqlQueryBuilder(options);
        }

        public R2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
            : this(databaseMetadataProvider, r2RMLConfiguration, new MappingOptions())
        {

        }

        /// <summary>
        /// R2RML graph's base URI
        /// </summary>
        public Uri MappingBaseUri { get; set; }

        public IDirectMappingStrategy MappingStrategy
        {
            get
            {
                if (_mappingStrategy == null)
                    _mappingStrategy = new DirectMappingStrategy(_options);
                return _mappingStrategy;
            }
            set { _mappingStrategy = value; }
        }

        public IColumnMappingStrategy ColumnMappingStrategy
        {
            get
            {
                if (_columnMappingStrategy == null)
                    _columnMappingStrategy = new ColumnMappingStrategy();
                return _columnMappingStrategy;
            }
            set { _columnMappingStrategy = value; }
        }

        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy(_options);
                return _primaryKeyMappingStrategy;
            }
            set { _primaryKeyMappingStrategy = value; }
        }

        public ISqlQueryBuilder SqlBuilder { get; set; }

        /// <summary>
        /// Generates default R2RML mappings based on database metadata
        /// </summary>
        public IR2RML GenerateMappings()
        {
            if (_databaseMetadataProvider.Tables != null)
                _databaseMetadataProvider.Tables.Accept(this);

            return _r2RMLConfiguration;
        }

        #region Implementation of IDatabaseMetadataVisitor

        public void Visit(TableCollection tables)
        {
        }

        public void Visit(TableMetadata table)
        {
            if (table.ForeignKeys.Any(fk => fk.IsCandidateKeyReference && fk.ReferencedTableHasPrimaryKey))
            {
                var r2RMLView = SqlBuilder.GetR2RMLViewForJoinedTables(table);
                CurrentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromR2RMLView(r2RMLView);
            }
            else
                CurrentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromTable(table.Name);

            if (table.PrimaryKey.Length == 0)
            {
                MappingStrategy.CreateSubjectMapForNoPrimaryKey(CurrentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
            else
            {
                MappingStrategy.CreateSubjectMapForPrimaryKey(CurrentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
        }

        public void Visit(ColumnMetadata column)
        {
            Uri predicateUri = ColumnMappingStrategy.CreatePredicateUri(MappingBaseUri, column);

            var propertyObjectMap = CurrentTriplesMapConfiguration.CreatePropertyObjectMap();
            propertyObjectMap.CreatePredicateMap().IsConstantValued(predicateUri);
            var literalTermMap = propertyObjectMap.CreateObjectMap().IsColumnValued(column.Name);

            var dataTypeUri = XsdDatatypes.GetDataType(column.Type);
            if (dataTypeUri != null)
                literalTermMap.HasDataType(dataTypeUri);
        }

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
