using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TCode.r2rml4net.RDB
{
    public class ColumnMetadata : IVistitable<IDatabaseMetadataVisitor>
    {
        public string Name { get; internal set; }
        public DbType Type { get; internal set; }
        public TableMetadata Table { get; internal set; }

        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
