using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDF
{
    public struct JoinCondition
    {
        string _childColumn;
        string _parentColumn;

        public JoinCondition(string childColumn, string parentColumn)
        {
            _childColumn = childColumn;
            _parentColumn = parentColumn;
        }

        public string ChildColumn { get { return _childColumn; } }
        public string ParentColumn { get { return _parentColumn; } }
    }
}
