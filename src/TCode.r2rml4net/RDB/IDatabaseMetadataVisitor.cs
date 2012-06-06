using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public interface IDatabaseMetadataVisitor
    {
        void Visit(TableCollection tables);
        void Visit(TableMetadata tables);
        void Visit(ColumnMetadata tables);
    }
}
