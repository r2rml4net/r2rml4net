#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation od <see cref="List{T}"/> and <see cref="IVistitable{T}"/> representing a group of tables
    /// </summary>
    public class TableCollection : IVistitable<IDatabaseMetadataVisitor>, IEnumerable<TableMetadata>
    {
        readonly List<TableMetadata> _tables = new List<TableMetadata>();

        /// <summary>
        /// Visits self and all contained tables
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (TableMetadata table in this)
                table.Accept(visitor);
        }

        /// <summary>
        /// Gets the table with the specified name
        /// </summary>
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public TableMetadata this[string tableName]
        {
            get
            {
                if (tableName == null)
                    throw new ArgumentNullException("tableName");
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentOutOfRangeException("tableName");

                var table = this.SingleOrDefault(t => t.Name == tableName);
                if (table == null)
                    throw new IndexOutOfRangeException(string.Format("TableCollection does not contain table {0}", tableName));

                return table;
            }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Gets enumerator for <see cref="IEnumerable{T}"/> of <see cref="TableMetadata"/>
        /// </summary>
        public IEnumerator<TableMetadata> GetEnumerator()
        {
            return _tables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        internal void Add(TableMetadata table)
        {
            if (this.Any(tab => tab.Name == table.Name))
                throw new ArgumentException(string.Format("TableCollection already contains a table named {0}", table.Name));

            _tables.Add(table);
        }

        /// <summary>
        /// Gets the table count
        /// </summary>
        public int Count
        {
            get { return _tables.Count; }
        }
    }
}
