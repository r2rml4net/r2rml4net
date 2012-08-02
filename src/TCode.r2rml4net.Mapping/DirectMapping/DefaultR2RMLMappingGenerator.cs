using System;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Builds a R2RML graph from a relational database's schema
    /// </summary>
    public class DefaultR2RMLMappingGenerator : IDatabaseMetadataVisitor
    {
        private readonly IDatabaseMetadata _databaseMetadataProvider;
        private readonly IR2RMLConfiguration _r2RMLConfiguration;
        private ITriplesMapConfiguration _currentTriplesMapConfiguration;
        private IDirectMappingStrategy _mappingStrategy;
        private IForeignKeyMappingStrategy _foreignKeyMappingStrategy;
        private IColumnMappingStrategy _columnMappingStrategy;
        private DirectMappingOptions _options;

        /// <summary>
        /// Creates <see cref="DefaultR2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public DefaultR2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration, DirectMappingOptions options)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._r2RMLConfiguration = r2RMLConfiguration;
            _options = options;

            MappingBaseUri = r2RMLConfiguration.BaseUri;
        }

        public DefaultR2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
            :this(databaseMetadataProvider, r2RMLConfiguration, new DirectMappingOptions())
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
                if(_mappingStrategy == null)
                    _mappingStrategy = new DefaultMappingStrategy(_options);
                return _mappingStrategy;
            }
            set { _mappingStrategy = value; }
        }

        public IForeignKeyMappingStrategy ForeignKeyMappingStrategy
        {
            get
            {
                if (_foreignKeyMappingStrategy == null)
                    _foreignKeyMappingStrategy = new DefaultForeignKeyMapping(_options); 
                return _foreignKeyMappingStrategy;
            }
            set { _foreignKeyMappingStrategy = value; }
        }

        public IColumnMappingStrategy ColumnMappingStrategy
        {
            get
            {
                if (_columnMappingStrategy == null)
                    _columnMappingStrategy = new DefaultColumnMapping();
                return _columnMappingStrategy;
            }
            set { _columnMappingStrategy = value; }
        }

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
            _currentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromTable(table.Name);

            if (table.PrimaryKey.Length == 0)
            {
                MappingStrategy.CreateSubjectMapForNoPrimaryKey(_currentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
            else
            {
                MappingStrategy.CreateSubjectMapForPrimaryKey(_currentTriplesMapConfiguration.SubjectMap, MappingBaseUri, table);
            }
        }

        public void Visit(ColumnMetadata column)
        {
            Uri predicateUri = ColumnMappingStrategy.CreatePredicateUri(MappingBaseUri, column);

            var propertyObjectMap = _currentTriplesMapConfiguration.CreatePropertyObjectMap();
            propertyObjectMap.CreatePredicateMap().IsConstantValued(predicateUri);
            var literalTermMap = propertyObjectMap.CreateObjectMap().IsColumnValued(column.Name);

            var dataTypeUri = XsdDatatypes.GetDataType(column.Type);
            if (dataTypeUri != null)
                literalTermMap.HasDataType(dataTypeUri);
        }

        public void Visit(ForeignKeyMetadata foreignKey)
        {
            var foreignKeyMap = _currentTriplesMapConfiguration.CreatePropertyObjectMap();

            Uri foreignKeyRefUri = 
                ForeignKeyMappingStrategy.CreateReferencePredicateUri(
                    MappingBaseUri,
                    foreignKey);

            foreignKeyMap.CreatePredicateMap()
                .IsConstantValued(foreignKeyRefUri);

            if (foreignKey.IsCandidateKeyReference)
            {
                foreignKeyMap.CreateObjectMap().TermType.IsBlankNode();
            }
            else
            {
                var templateForForeignKey = ForeignKeyMappingStrategy.CreateReferenceObjectTemplate(MappingBaseUri, foreignKey);

                foreignKeyMap.CreateObjectMap()
                    .IsTemplateValued(templateForForeignKey);
            }
        }

        #endregion
    }
}
