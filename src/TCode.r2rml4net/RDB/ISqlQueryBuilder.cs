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
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Contract for classes building effective sql queries for <see cref="ITriplesMap"/> and <see cref="IRefObjectMap"/>
    /// </summary>
    public interface ISqlQueryBuilder
    {
        /// <summary>
        /// Gets effective sql query based on <see cref="ITriplesMap.TableName"/> or <see cref="ITriplesMap.SqlQuery"/>
        /// </summary>
        string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap);

        /// <summary>
        /// Gets effective sql query based on parent/child <see cref="IQueryMap.EffectiveSqlQuery"/> maps and <see cref="IRefObjectMap.JoinConditions"/> (if any)
        /// </summary>
        string GetEffectiveQueryForRefObjectMap(IRefObjectMap refObjectMap);

        /// <summary>
        /// Gets the SQL query for tables joined by candidate key where the parent table has a primary key
        /// </summary>
        /// <param name="table">child table</param>
        string GetR2RMLViewForJoinedTables(TableMetadata table);
    }
}