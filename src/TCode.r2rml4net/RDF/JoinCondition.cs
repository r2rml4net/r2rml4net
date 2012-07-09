using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// An optional join condition between triple maps
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-join-condition</remarks>
    public struct JoinCondition
    {
        string _childColumn;
        string _parentColumn;

        /// <summary>
        /// Creates an instance of <see cref="JoinCondition"/>
        /// </summary>
        /// <param name="childColumn">See http://www.w3.org/TR/r2rml/#dfn-child-column</param>
        /// <param name="parentColumn">See http://www.w3.org/TR/r2rml/#dfn-parent-column</param>
        public JoinCondition(string childColumn, string parentColumn)
        {
            _childColumn = childColumn;
            _parentColumn = parentColumn;
        }

        /// <summary>
        /// Gets the child column, which will be used to create joins between logical tables
        /// </summary>
        /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-joint-sql-query</remarks>
        public string ChildColumn { get { return _childColumn; } }
        /// <summary>
        /// Gets the parent column, which will be used to create joins between logical tables
        /// </summary>
        /// <remarks>Read more on http://www.w3.org/TR/r2rml/#dfn-joint-sql-query</remarks>
        public string ParentColumn { get { return _parentColumn; } }
    }
}
