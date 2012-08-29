#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a foreign key
    /// </summary>
    public class ForeignKeyMetadata
    {
        /// <summary>
        /// Gets the table containing this foreign key
        /// </summary>
        public string TableName { get; internal set; }
        /// <summary>
        /// Gets the foreign key column
        /// </summary>
        public string[] ForeignKeyColumns { get; internal set; }
        /// <summary>
        /// Get the referenced table
        /// </summary>
        public TableMetadata ReferencedTable { get; internal set; }
        /// <summary>
        /// Gets the referenced columns
        /// </summary>
        public string[] ReferencedColumns { get; internal set; }
        /// <summary>
        /// Returns true if the referenced columns do not form a primary key
        /// </summary>
        public bool IsCandidateKeyReference { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether the referenced table has primary key
        /// </summary>
        public bool ReferencedTableHasPrimaryKey { get; internal set; }
    }
}