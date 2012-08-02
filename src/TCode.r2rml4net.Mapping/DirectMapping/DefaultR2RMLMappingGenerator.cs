using System;
using System.Linq;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

#pragma warning disable

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

        /// <summary>
        /// Creates <see cref="DefaultR2RMLMappingGenerator"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public DefaultR2RMLMappingGenerator(IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._r2RMLConfiguration = r2RMLConfiguration;

            MappingBaseUri = r2RMLConfiguration.BaseUri;
            MappingStrategy = new DefaultMappingStrategy();
        }

        /// <summary>
        /// R2RML graph's base URI
        /// </summary>
        public Uri MappingBaseUri { get; set; }

        public IDirectMappingStrategy MappingStrategy { get; set; }

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

            var classIri = MappingStrategy.CreateSubjectUri(MappingBaseUri, table.Name);
            if (table.PrimaryKey.Length == 0)
            {
                string template = MappingStrategy.CreateSubjectTemplateForNoPrimaryKey(
                    table.Name, 
                    table.Select(col => col.Name));

                // empty primary key generates blank node subjects
                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .TermType.IsBlankNode()
                    .IsTemplateValued(template);
            }
            else
            {
                string template = MappingStrategy.CreateSubjectTemplateForPrimaryKey(
                    MappingBaseUri,
                    table.Name,
                    table.PrimaryKey.Select(pk => pk.Name));

                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .IsTemplateValued(template);
            }
        }

        public void Visit(ColumnMetadata column)
        {
            Uri predicateUri = MappingStrategy.CreatePredicateUri(MappingBaseUri, column.Table.Name, column.Name);

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
                MappingStrategy.CreateReferencePredicateUri(
                    MappingBaseUri,
                    foreignKey.TableName,
                    foreignKey.ForeignKeyColumns);

            foreignKeyMap.CreatePredicateMap()
                .IsConstantValued(foreignKeyRefUri);

            if (foreignKey.IsCandidateKeyReference)
            {
                foreignKeyMap.CreateObjectMap().TermType.IsBlankNode();
            }
            else
            {
                var templateForForeignKey = MappingStrategy.CreateReferenceObjectTemplate(
                    MappingBaseUri,
                    foreignKey.ReferencedTableName,
                    foreignKey.ForeignKeyColumns,
                    foreignKey.ReferencedColumns);

                foreignKeyMap.CreateObjectMap()
                    .IsTemplateValued(templateForForeignKey);
            }
        }

        #endregion
    }
}
