﻿#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a table from a relational database
    /// </summary>
    public class TableMetadata : ColumnCollection, IVistitable<IDatabaseMetadataVisitor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableMetadata"/> class.
        /// </summary>
        public TableMetadata()
        {
            ForeignKeys = new ForeignKeyMetadata[0];
            UniqueKeys = new UniqueKeyCollection();
        }

        /// <summary>
        /// Gets the collection of table's unique keys
        /// </summary>
        public UniqueKeyCollection UniqueKeys { get; internal set; }

        /// <summary>
        /// Gets the table name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the primary key column (or columns in case of composite key)
        /// </summary>
        public string[] PrimaryKey
        {
            get { return this.Where(c => c.IsPrimaryKey).Select(c => c.Name).ToArray(); }
        }

        /// <summary>
        /// Gets all the foreign keys
        /// </summary>
        public ForeignKeyMetadata[] ForeignKeys { get; internal set; }

        /// <summary>
        /// Visits self and contained columns
        /// </summary>
        public void Accept(IDatabaseMetadataVisitor visitor)
        {
            visitor.Visit(this);

            foreach (ColumnMetadata column in this)
            {
                column.Accept(visitor);
            }

            if (this.ForeignKeys != null)
            {
                foreach (var foreignKey in ForeignKeys)
                {
                    visitor.Visit(foreignKey);
                }
            }
        }

        /// <summary>
        /// Adds a column to table
        /// </summary>
        protected internal override void Add(ColumnMetadata column)
        {
            base.Add(column);
            column.Table = this;
        }
  }
}
