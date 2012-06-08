using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using VDS.RDF;
using System.Data;

#pragma warning disable

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Builds a R2RML graph from a relational database's schema
    /// </summary>
    public class DirectMappingR2RMLBuilder : IDatabaseMetadataVisitor
    {
        private VDS.RDF.IGraph _r2rmlGraph;

        private RDB.IDatabaseMetadata _databaseMetadataProvider;
        private IR2RMLConfiguration _R2RMLConfiguration;

        /// <summary>
        /// Creates <see cref="DirectMappingR2RMLBuilder"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public DirectMappingR2RMLBuilder(RDB.IDatabaseMetadata databaseMetadataProvider, IR2RMLConfiguration r2RMLConfiguration)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._R2RMLConfiguration = _R2RMLConfiguration;

            MappingBaseUri = new Uri("http://mappingpedia.org/rdb2rdf/r2rml/tc/");
            MappedDataBaseUri = new Uri("http://example.com/");

            BuildEmptyGraph();
        }

        public Uri MappingBaseUri { get; private set; }
        public Uri MappedDataBaseUri { get; private set; }

        private void BuildEmptyGraph()
        {
            _r2rmlGraph = new VDS.RDF.Graph();

            _r2rmlGraph.BaseUri = MappingBaseUri;
            _r2rmlGraph.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
        }

        /// <summary>
        /// Returns an R2RML graph generated for direct mapping
        /// </summary>
        [Obsolete]
        public VDS.RDF.IGraph R2RMLGraph
        {
            get
            {
                return _r2rmlGraph;
            }
        }

        public void BuildGraph()
        {
            if (_databaseMetadataProvider.Tables != null)
                _databaseMetadataProvider.Tables.Accept(this);
        }

        #region Implementation of IDatabaseMetadataVisitor

        private ITriplesMapConfiguration currentTriplesMapConfiguration;
        private IUriNode currentTripleMap;

        public void Visit(TableCollection tables)
        {
        }

        public void Visit(TableMetadata table)
        {
            currentTripleMap = R2RMLGraph.CreateUriNode(new Uri(string.Format("{0}TriplesMap", table.Name), UriKind.Relative));

            AssertSubjectMapTriples(table);
        }

        private void AssertSubjectMapTriples(TableMetadata table)
        {
            currentTriplesMapConfiguration = _R2RMLConfiguration.CreateTriplesMapFromTable(table.Name);

            var subjectMap = R2RMLGraph.CreateUriNode("rr:subjectMap");
            var rrClass = R2RMLGraph.CreateUriNode("rr:class");
            var rrTemplate = R2RMLGraph.CreateUriNode("rr:template");
            var termType = R2RMLGraph.CreateUriNode("rr:termType");
            var blankNode = R2RMLGraph.CreateUriNode("rr:BlankNode");
            var template = R2RMLGraph.CreateLiteralNode(string.Format("{0}{1}", this.MappedDataBaseUri, table.Name));
            var subjectMapDef = R2RMLGraph.CreateBlankNode();
            var classDef = R2RMLGraph.CreateBlankNode();

            R2RMLGraph.Assert(currentTripleMap, subjectMap, subjectMapDef);
            R2RMLGraph.Assert(subjectMapDef, termType, blankNode);
            R2RMLGraph.Assert(subjectMapDef, rrClass, classDef);
            R2RMLGraph.Assert(classDef, rrTemplate, template);
        }

        public void Visit(ColumnMetadata column)
        {
            AssertPredicateObjectMap(column);
        }

        private void AssertPredicateObjectMap(ColumnMetadata column)
        {
            var predicateObjectMap = R2RMLGraph.CreateUriNode("rr:predicateObjectMap");
            var predicate = R2RMLGraph.CreateUriNode("rr:predicate");
            var objectMap = R2RMLGraph.CreateUriNode("rr:objectMap");
            var rrColumn = R2RMLGraph.CreateUriNode("rr:column");
            var rrTemplate = R2RMLGraph.CreateUriNode("rr:template");
            var dataType = R2RMLGraph.CreateUriNode("rr:dataType");
            var predicateTemplate = R2RMLGraph.CreateLiteralNode(string.Format("{0}{1}#{2}", MappedDataBaseUri, column.Table.Name, column.Name));
            var columnName = R2RMLGraph.CreateLiteralNode(column.Name);
            var predicateDef = R2RMLGraph.CreateBlankNode();
            var objectMapDef = R2RMLGraph.CreateBlankNode();
            var predicateObjectMapDef = R2RMLGraph.CreateBlankNode();
            var literalType = LiterlUriNode(column);

            R2RMLGraph.Assert(currentTripleMap, predicateObjectMap, predicateObjectMapDef);
            R2RMLGraph.Assert(predicateObjectMapDef, predicate, predicateDef);
            R2RMLGraph.Assert(predicateObjectMapDef, objectMap, objectMapDef);
            R2RMLGraph.Assert(predicateDef, rrTemplate, predicateTemplate);
            R2RMLGraph.Assert(objectMapDef, rrColumn, columnName);
            if (literalType != null)
                R2RMLGraph.Assert(objectMapDef, dataType, literalType);
        }

        #endregion

        private IUriNode LiterlUriNode(ColumnMetadata column)
        {
            switch (column.Type)
            {
                case System.Data.DbType.Int16:
                    return R2RMLGraph.CreateUriNode("xsd:short");
                case System.Data.DbType.Int32:
                    return R2RMLGraph.CreateUriNode("xsd:int");
                case System.Data.DbType.Boolean:
                    return R2RMLGraph.CreateUriNode("xsd:boolean");
                case System.Data.DbType.Byte:
                    return R2RMLGraph.CreateUriNode("xsd:unsignedByte");
                case System.Data.DbType.Date:
                    return R2RMLGraph.CreateUriNode("xsd:date");
                case System.Data.DbType.DateTime:
                case System.Data.DbType.DateTime2:
                    return R2RMLGraph.CreateUriNode("xsd:datetime");
                case System.Data.DbType.Currency:
                    return R2RMLGraph.CreateUriNode("xsd:double");
                case System.Data.DbType.DateTimeOffset:
                    return R2RMLGraph.CreateUriNode("xsd:datetime");
                case System.Data.DbType.Decimal:
                    return R2RMLGraph.CreateUriNode("xsd:decimal");
                case System.Data.DbType.Double:
                    return R2RMLGraph.CreateUriNode("xsd:double");
                case System.Data.DbType.SByte:
                    return R2RMLGraph.CreateUriNode("xsd:byte");
                case System.Data.DbType.Single:
                    return R2RMLGraph.CreateUriNode("xsd:float");
                case System.Data.DbType.Time:
                    return R2RMLGraph.CreateUriNode("xsd:time");
                case System.Data.DbType.UInt16:
                    return R2RMLGraph.CreateUriNode("xsd:unsignedShort");
                case System.Data.DbType.UInt32:
                    return R2RMLGraph.CreateUriNode("xsd:unsignedInt");
                case System.Data.DbType.UInt64:
                    return R2RMLGraph.CreateUriNode("xsd:unsignedLong");
                default:
                    return null;
            }
        }
    }
}
