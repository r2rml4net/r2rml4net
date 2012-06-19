using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using System.Web;

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

            var classIri = CreateUriForTable(table.Name);
            if (table.PrimaryKey.Length == 0)
            {
                // empty primary key generates blank node subjects
                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .TermType.IsBlankNode();
            }
            else
            {
                string template = CreateTemplateForPrimaryKey(table.Name, table.PrimaryKey.Select(pk => pk.Name));

                _currentTriplesMapConfiguration.SubjectMap
                    .AddClass(classIri)
                    .IsTemplateValued(template);
            }
        }

        public void Visit(ColumnMetadata column)
        {
            string predicateUriString = string.Format("{0}{1}#{2}", this.MappedDataBaseUri, column.Table.Name, column.Name);
            Uri predicateUri = new Uri(predicateUriString);

            var propertyObjectMap = _currentTriplesMapConfiguration.CreatePropertyObjectMap();
            propertyObjectMap.CreatePredicateMap().IsConstantValued(predicateUri);
            var literalTermMap = propertyObjectMap.CreateObjectMap().IsColumnValued(column.Name);

            var dataTypeUri = UrisHelper.GetXsdDataType(column.Type);
            if(dataTypeUri != null)
                literalTermMap.HasDataType(dataTypeUri);
        }

        public void Visit(ForeignKeyMetadata foreignKey)
        {
            var foreignKeyMap = _currentTriplesMapConfiguration.CreatePropertyObjectMap();

            Uri foreignKeyRefUri = CreateUriForReferenceProperty(foreignKey.TableName, foreignKey.ForeignKeyColumns);
            foreignKeyMap.CreatePredicateMap()
                .IsConstantValued(foreignKeyRefUri);

            if (foreignKey.IsCandidateKeyReference)
            {
                foreignKeyMap.CreateObjectMap().TermType.IsBlankNode();
            }
            else
            {
                var templateForForeignKey = CreateTemplateForForeignKey(foreignKey.ReferencedTableName,
                                                                        foreignKey.ForeignKeyColumns,
                                                                        foreignKey.ReferencedColumns);
                foreignKeyMap.CreateObjectMap()
                    .IsTemplateValued(UrlEncode(templateForForeignKey));
            }
        }

        #endregion

        private Uri CreateUriForTable(string name)
        {
            return new Uri(this.MappedDataBaseUri + UrlEncode(name));
        }

        private Uri CreateUriForReferenceProperty(string tableName, IEnumerable<string> foreignKey)
        {
            string uri = this.MappedDataBaseUri + UrlEncode(tableName) + "#ref-" + string.Join(".", foreignKey.Select(UrlEncode));

            return new Uri(UrlEncode(uri));
        }

        private string CreateTemplateForPrimaryKey(string tableName, IEnumerable<string> primaryKey)
        {
            string template = UrlEncode(CreateUriForTable(tableName).ToString());
            template += "/" + string.Join(";", primaryKey.Select(pk => string.Format("{0}={{{1}}}", UrlEncode(pk), pk)));
            return template;
        }

        private string CreateTemplateForForeignKey(string tableName, IEnumerable<string> foreignKey, IEnumerable<string> referencedPrimaryKey)
        {
            foreignKey = foreignKey.ToArray();
            referencedPrimaryKey = referencedPrimaryKey.ToArray();

            if(foreignKey.Count() != referencedPrimaryKey.Count())
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", tableName), "foreignKey");

            if(!foreignKey.Any())
                throw new ArgumentException("Empty foreign key", "foreignKey");

            StringBuilder template = new StringBuilder(CreateUriForTable(tableName) + "/");
            template.AppendFormat("{0}={{{1}}}", UrlEncode(referencedPrimaryKey.ElementAt(0)), foreignKey.ElementAt(0));
            for (int i = 1; i < foreignKey.Count(); i++)
            {
                template.AppendFormat(";{0}={{{1}}}", UrlEncode(referencedPrimaryKey.ElementAt(1)), foreignKey.ElementAt(1));
            }
            return template.ToString();
        }

        string UrlEncode(string unescapedString)
        {
            return HttpUtility.UrlDecode(unescapedString);
        }
    }
}
