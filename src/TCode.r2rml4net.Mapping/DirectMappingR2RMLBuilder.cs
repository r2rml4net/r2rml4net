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
    public class DirectMappingR2RMLBuilder
    {
        private RDB.IDatabaseMetadata _databaseMetadataProvider;
        private IDatabaseMetadataVisitor _databaseMetadataVisitor;

        /// <summary>
        /// Creates <see cref="DirectMappingR2RMLBuilder"/> which will read RDB metadata using <see cref="RDB.IDatabaseMetadata"/>
        /// </summary>
        public DirectMappingR2RMLBuilder(RDB.IDatabaseMetadata databaseMetadataProvider, IDatabaseMetadataVisitor databaseMetadataVisitor)
        {
            this._databaseMetadataProvider = databaseMetadataProvider;
            this._databaseMetadataVisitor = databaseMetadataVisitor;
        }

        /// <summary>
        /// Returns an R2RML graph generated for direct mapping
        /// </summary>
        public VDS.RDF.IGraph R2RMLGraph
        {
            get
            {
                return null;
            }
        }

        public VDS.RDF.IGraph BuildGraph()
        {
            throw new NotImplementedException();
        }
    }
}
