using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableMetadata : IVistitable<IDatabaseMetadataVisitor>
    {
        public IEnumerable<ColumnMetadata> Columns { get; internal set; }

        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
