using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Interface for the Visitor pattern, which visits tables and columns to extract 
    /// relational database structure
    /// </summary>
    public interface IDatabaseMetadataVisitor
    {
        /// <summary>
        /// Visits a <see cref="TableCollection"/> and it's tables
        /// </summary>
        void Visit(TableCollection tables);
        /// <summary>
        /// Visits a <see cref="TableMetadata"/> and it's columns
        /// </summary>
        void Visit(TableMetadata table);
        /// <summary>
        /// Visits a <see cref="ColumnMetadata"/>
        /// </summary>
        void Visit(ColumnMetadata column);
    }
}
