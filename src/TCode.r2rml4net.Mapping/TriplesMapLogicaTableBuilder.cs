using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.Mapping
{
    public class TriplesMapLogicalTableBuilder
    {
        public LogicalTableBuilder FromTableOrView(string tableName)
        {
            throw new NotImplementedException();
        }

        public LogicalTableBuilder FromSqlQuery(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class LogicalTableBuilder
    {
        public LogicalTableBuilder SqlVersion(Uri sqlVersionUri)
        {
            throw new NotImplementedException();
        }

        public TriplesMapSubjectMapBuilder SubjectMap()
        {
            throw new NotImplementedException();
        }
    }
}
