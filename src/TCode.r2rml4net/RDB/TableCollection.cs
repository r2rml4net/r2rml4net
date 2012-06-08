using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation od <see cref="List{T}"/> and <see cref="IVistitable{T}"/> representing a group of tables
    /// </summary>
    public class TableCollection : List<TableMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        /// <summary>
        /// Visits self and all contained tables
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (TableMetadata table in this)
                table.Accept(visitor);
        }
    }
}
