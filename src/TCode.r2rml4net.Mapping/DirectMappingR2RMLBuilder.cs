using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Builds a R2RML graph from a relational database's schema
    /// </summary>
    public class DirectMappingR2RMLBuilder : IDatabaseMetadataVisitor
    {
        private VDS.RDF.IGraph _r2rmlGraph;

        private RDB.IDatabaseMetadata _databaseMetadataProvider;

        /// <summary>
        /// Creates <see cref="DirectMappingR2RMLBuilder"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public DirectMappingR2RMLBuilder(RDB.IDatabaseMetadata databaseMetadataProvider)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;

            MappingBaseUri = new Uri("http://mappingpedia.org/rdb2rdf/r2rml/tc/");
            MappedDataBaseUri = new Uri("http://example.com/");

            BuildEmptyGraph();
        }

        public Uri MappingBaseUri { get; private set; }
        public Uri MappedDataBaseUri { get; private set; }

        private void BuildEmptyGraph()
        {
            _r2rmlGraph = new VDS.RDF.Graph(true);

            _r2rmlGraph.BaseUri = MappingBaseUri;
            _r2rmlGraph.NamespaceMap.AddNamespace("xsd", new Uri("http://www.w3.org/2001/XMLSchema#"));
            _r2rmlGraph.NamespaceMap.AddNamespace("rr", new Uri("http://www.w3.org/ns/r2rml#"));
        }

        /// <summary>
        /// Returns an R2RML graph generated for direct mapping
        /// </summary>
        public VDS.RDF.IGraph R2RMLGraph
        {
            get
            {
                return _r2rmlGraph;
            }
        }

        public void BuildGraph()
        {
            _databaseMetadataProvider.Tables.Accept(this);
        }

        #region Implementation of IDatabaseMetadataVisitor

        public void Visit(TableCollection tables)
        {
            throw new NotImplementedException();
        }

        public void Visit(TableMetadata table)
        {
            throw new NotImplementedException();
        }

        public void Visit(ColumnMetadata column)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
