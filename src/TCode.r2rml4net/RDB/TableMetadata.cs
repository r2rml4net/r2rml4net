using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableMetadata : List<ColumnMetadata>, ICollection<ColumnMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        public string Name { get; internal set; }

        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            throw new NotImplementedException();
        }

        void ICollection<ColumnMetadata>.Add(ColumnMetadata column)
        {
            column.Table = this;
        }
    }
}
