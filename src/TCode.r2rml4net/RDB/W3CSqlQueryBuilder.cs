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
        private readonly MappingOptions _options;

        public W3CSqlQueryBuilder(MappingOptions options)
        {
            _options = options;
            SqlVersionValidator = new Wc3SqlVersionValidator();
        }

        public W3CSqlQueryBuilder()
            : this(new MappingOptions())
        {
        }

        public ISqlVersionValidator SqlVersionValidator { get; set; }

        #region Implementation of ISqlQueryBuilder

        /// <summary>
        /// Gets effective sql query based on table name or sql view
        /// </summary>
        /// <returns>SQL query string</returns>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        public string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap)
        {
            if (_options.ValidateSqlVersion && !triplesMap.SqlVersions.All(SqlVersionValidator.SqlVersionIsValid))
                throw new InvalidSqlVersionException(triplesMap.SqlVersions.First(version => !SqlVersionValidator.SqlVersionIsValid(version)));

            if (triplesMap.TableName != null && triplesMap.SqlQuery != null)
            {
                throw new InvalidMapException("Triples map cannot have both table name and sql query set");
            }

            if (triplesMap.TableName != null)
                return string.Format("SELECT * FROM {0}", DatabaseIdentifiersHelper.DelimitIdentifier(triplesMap.TableName, _options));

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
                            var childColumn = DatabaseIdentifiersHelper.DelimitIdentifier(@join.ChildColumn, _options);
                            var parentColumn = DatabaseIdentifiersHelper.DelimitIdentifier(@join.ParentColumn, _options);
                            return string.Format("child.{0}=parent.{1}", childColumn, parentColumn);
                        });

                return string.Format(@"SELECT * FROM ({0}) AS child, 
({1}) AS parent
WHERE {2}", refObjectMap.ChildEffectiveSqlQuery, refObjectMap.ParentEffectiveSqlQuery, string.Join("\nAND ", joinStatements));
            }

            return string.Format("SELECT * FROM ({0}) AS tmp", refObjectMap.ChildEffectiveSqlQuery);
        }

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
            sqlBuilder.AppendFormat("FROM {0} as child", DatabaseIdentifiersHelper.DelimitIdentifier(table.Name, _options));
            sqlBuilder.AppendLine();

            int i = 1;
            foreach (var foreignKey in fkTargetHasPrimaryKey)
            {
                sqlBuilder.AppendFormat("LEFT JOIN {0} as p{1} ON",
                    DatabaseIdentifiersHelper.DelimitIdentifier(foreignKey.ReferencedTable.Name, _options),
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
                                        DatabaseIdentifiersHelper.DelimitIdentifier(pkColumn.Name, _options),
                                        DatabaseIdentifiersHelper.DelimitIdentifier(foreignKey.ReferencedTable.Name + pkColumn.Name, _options));
        }

        private IEnumerable<string> GetJoinConditions(ForeignKeyMetadata foreignKey, string parent)
        {
            return foreignKey.ReferencedColumns
                .Select((t, colIdx) => string.Format("{0}.{1} = child.{2}",
                                                     parent,
                                                     DatabaseIdentifiersHelper.DelimitIdentifier(
                                                         t,
                                                         _options
                                                     ),
                                                     DatabaseIdentifiersHelper.DelimitIdentifier(
                                                         foreignKey.ForeignKeyColumns[colIdx],
                                                         _options
                                                     )));
        }
    }
}