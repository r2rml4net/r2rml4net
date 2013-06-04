#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation of <see cref="ISqlQueryBuilder"/>, which returns effective sql query as described on R2RML specifications page
    /// </summary>
    public class W3CSqlQueryBuilder : ISqlQueryBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="W3CSqlQueryBuilder"/>
        /// </summary>
        public W3CSqlQueryBuilder()
        {
            SqlVersionValidator = new Wc3SqlVersionValidator();
        }

        /// <summary>
        /// Validates SQL version of R2RML views
        /// </summary>
        public ISqlVersionValidator SqlVersionValidator { get; set; }

        #region Implementation of ISqlQueryBuilder

        /// <summary>
        /// Gets effective sql query based on table name or sql view
        /// </summary>
        /// <returns>SQL query string</returns>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        public string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap)
        {
            if (MappingOptions.Current.ValidateSqlVersion && !triplesMap.SqlVersions.All(SqlVersionValidator.SqlVersionIsValid))
                throw new InvalidSqlVersionException(triplesMap.SqlVersions.First(version => !SqlVersionValidator.SqlVersionIsValid(version)));

            if (triplesMap.TableName != null && triplesMap.SqlQuery != null)
            {
                throw new InvalidMapException("Triples map cannot have both table name and sql query set");
            }

            if (triplesMap.TableName != null)
                return string.Format("SELECT * FROM {0}", DatabaseIdentifiersHelper.DelimitIdentifier(triplesMap.TableName));

            return triplesMap.SqlQuery;
        }

        /// <summary>
        /// Gets effective sql query based on parent/child triples maps and join conditions (if any)
        /// </summary>
        /// <returns>SQL query string</returns>
        /// <remarks>See http://www.w3.org/TR/r2rml/#foreign-key</remarks>
        public string GetEffectiveQueryForRefObjectMap(IRefObjectMap refObjectMap)
        {
            if (refObjectMap.JoinConditions.Any())
            {
                var joinStatements =
                    refObjectMap.JoinConditions.Select(
                        delegate(JoinCondition @join)
                        {
                            var childColumn = DatabaseIdentifiersHelper.DelimitIdentifier(@join.ChildColumn);
                            var parentColumn = DatabaseIdentifiersHelper.DelimitIdentifier(@join.ParentColumn);
                            return string.Format("child.{0}=parent.{1}", childColumn, parentColumn);
                        });

                return string.Format(@"SELECT * FROM ({0}) AS child, 
({1}) AS parent
WHERE {2}", refObjectMap.ChildEffectiveSqlQuery, refObjectMap.ParentEffectiveSqlQuery, string.Join("\nAND ", joinStatements));
            }

            return string.Format("SELECT * FROM ({0}) AS tmp", refObjectMap.ChildEffectiveSqlQuery);
        }

        /// <summary>
        /// Gets the SQL query for tables joined by candidate key where the parent table has a primary key
        /// </summary>
        /// <param name="table">child table</param>
        public string GetR2RMLViewForJoinedTables(TableMetadata table)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (table.ForeignKeys.All(fk => !fk.ReferencedTableHasPrimaryKey))
                throw new ArgumentException("None of the referenced tables have a primary key", "table");

            StringBuilder sqlBuilder = new StringBuilder();

            var fkTargetHasPrimaryKey = table.ForeignKeys.Where(fk => fk.ReferencedTableHasPrimaryKey).ToArray();

            sqlBuilder.AppendFormat("SELECT child.*, {0}", string.Join(", ", GetJoinedPrimaryKeyColumnList(fkTargetHasPrimaryKey)));
            sqlBuilder.AppendLine();
            sqlBuilder.AppendFormat("FROM {0} as child", DatabaseIdentifiersHelper.DelimitIdentifier(table.Name));
            sqlBuilder.AppendLine();

            int i = 1;
            foreach (var foreignKey in fkTargetHasPrimaryKey)
            {
                sqlBuilder.AppendFormat("LEFT JOIN {0} as p{1} ON",
                    DatabaseIdentifiersHelper.DelimitIdentifier(foreignKey.ReferencedTable.Name),
                    i);
                sqlBuilder.AppendLine();
                sqlBuilder.Append(string.Join(" AND ", GetJoinConditions(foreignKey, "p" + i++)));
                sqlBuilder.AppendLine();
            }

            return sqlBuilder.ToString();
        }

        #endregion

        private IEnumerable<string> GetJoinedPrimaryKeyColumnList(IEnumerable<ForeignKeyMetadata> foreignKeys)
        {
            int i = 1;
            return from foreignKey in foreignKeys
                   let tableAlias = "p" + i++
                   from pkColumn in foreignKey.ReferencedTable.PrimaryKey
                   select string.Format("{0}.{1} as {2}",
                                        tableAlias,
                                        DatabaseIdentifiersHelper.DelimitIdentifier(pkColumn),
                                        DatabaseIdentifiersHelper.DelimitIdentifier(foreignKey.ReferencedTable.Name + pkColumn));
        }

        private IEnumerable<string> GetJoinConditions(ForeignKeyMetadata foreignKey, string parent)
        {
            return foreignKey.ReferencedColumns
                .Select((t, colIdx) => string.Format("{0}.{1} = child.{2}",
                                                     parent,
                                                     DatabaseIdentifiersHelper.DelimitIdentifier(
                                                         t
                                                     ),
                                                     DatabaseIdentifiersHelper.DelimitIdentifier(
                                                         foreignKey.ForeignKeyColumns[colIdx]
                                                     )));
        }
    }
}