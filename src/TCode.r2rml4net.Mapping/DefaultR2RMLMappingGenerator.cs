using System;
using System.Linq;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;

#pragma warning disable

namespace TCode.r2rml4net.Mapping
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

            MappingBaseUri = new Uri("http://mappingpedia.org/rdb2rdf/r2rml/tc/");
            MappedDataBaseUri = new Uri("http://example.com/");
        }

        /// <summary>
        /// R2RML graph's base URI
        /// </summary>
        public Uri MappingBaseUri { get; private set; }
        /// <summary>
        /// Base URI used to generate triples' subjects
        /// </summary>
        public Uri MappedDataBaseUri { get; private set; }

        /// <summary>
        /// Generates default R2RML mappings based on database metadata
        /// </summary>
        public void GenerateMappings()
        {
            if (_databaseMetadataProvider.Tables != null)
                _databaseMetadataProvider.Tables.Accept(this);
        }

        #region Implementation of IDatabaseMetadataVisitor

        public void Visit(TableCollection tables)
        {
        }

        public void Visit(TableMetadata table)
        {
            _currentTriplesMapConfiguration = _r2RMLConfiguration.CreateTriplesMapFromTable(table.Name);

            var classIri = new Uri(this.MappedDataBaseUri + table.Name);
            if (table.PrimaryKey.Length == 0)
            {
                // empty primary key generates blank node subjects
                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .TermType.IsBlankNode();
            }
            else
            {
                string template = classIri.ToString();
                template += "/" + string.Join(";", table.PrimaryKey.Select(pk => string.Format("{0}={{{0}}}", pk.Name)));

                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .IsTemplateValued(template);
            }
        }

        public void Visit(ColumnMetadata column)
        {
            Uri predicateUri = new Uri(string.Format("{0}{1}#{2}", this.MappedDataBaseUri, column.Table.Name, column.Name));

            var propertyObjectMap = _currentTriplesMapConfiguration.CreatePropertyObjectMap();
            propertyObjectMap.CreatePredicateMap().IsConstantValued(predicateUri);
            var literalTermMap = propertyObjectMap.CreateObjectMap().IsColumnValued(column.Name);

            var dataTypeUri = UrisHelper.GetXsdDataType(column.Type);
            if(dataTypeUri != null)
                literalTermMap.HasDataType(dataTypeUri);
        }     

        #endregion
    }
}
