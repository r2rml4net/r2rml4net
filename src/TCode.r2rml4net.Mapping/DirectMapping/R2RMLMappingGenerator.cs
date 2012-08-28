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
        /// Creates <see cref="R2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/> and custom mapping options
        /// </summary>
        public R2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration, MappingOptions options)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._r2RMLConfiguration = r2RMLConfiguration;
            _options = options;

            MappingBaseUri = r2RMLConfiguration.BaseUri;
            SqlBuilder = new W3CSqlQueryBuilder(options);
        }

        /// <summary>
        /// Creates <see cref="R2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/> and default mapping options
        /// </summary>
        public R2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
            : this(databaseMetadataProvider, r2RMLConfiguration, new MappingOptions())
        {

        }

        /// <summary>
        /// R2RML graph's base URI
        /// </summary>
        public Uri MappingBaseUri { get; set; }

        /// <summary>
        /// Implementation of <see cref="IDirectMappingStrategy"/>, which defines how to map relational database to RDF subject , predicate and object maps
        /// </summary>
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

        /// <summary>
        /// Impementation of <see cref="IColumnMappingStrategy"/>, which defines how to map columns to RDF predicates
        /// </summary>
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

        /// <summary>
        /// Implementation of <see cref="IPrimaryKeyMappingStrategy"/>, which defines how to map primary keys to RDF subjects
        /// </summary>
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

        /// <summary>
        /// Implementation of <see cref="ISqlQueryBuilder"/>, which builds queries used to retrieve data from relationalt database for genertaing triples
        /// </summary>
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
                literalTermMap.HasDataType(dataTypeUri);
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
