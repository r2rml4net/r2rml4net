using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDB
{
    public class TableMetadata : IEnumerable<ColumnMetadata>, IVistitable<IDatabaseMetadataVisitor>
    {
        private readonly IList<ColumnMetadata> _columns = new List<ColumnMetadata>();

        public string Name { get; internal set; }

        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (ColumnMetadata column in this)
                column.Accept(visitor);
        }

        public IEnumerator<ColumnMetadata> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        public void Add(ColumnMetadata column)
        {
            column.Table = this;
            _columns.Add(column);
        }
    }
}
