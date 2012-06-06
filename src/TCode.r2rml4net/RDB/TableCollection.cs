using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableCollection : List<TableMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
