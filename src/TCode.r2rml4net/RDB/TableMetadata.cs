using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableMetadata : List<ColumnMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
