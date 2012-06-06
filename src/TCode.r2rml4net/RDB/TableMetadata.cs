using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableMetadata
    {
        public IEnumerable<ColumnMetadata> Columns { get; internal set; }
    }
}
