using System.Linq;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Implementation of <see cref="IEffectiveSqlBuilder"/>, which returns effective sql query as described on R2RML specifications page
    /// </summary>
    public class W3CEffectiveSqlBuilder : IEffectiveSqlBuilder
    {
        #region Implementation of IEffectiveSqlBuilder

        /// <summary>
        /// Gets effective sql query based on table name or sql view
        /// </summary>
        /// <returns>SQL query string</returns>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        public string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap)
        {
            if (triplesMap.TableName != null && triplesMap.SqlQuery != null)
            {
                throw new InvalidTriplesMapException("Triples map cannot have both table name and sql query set");
            }

            if (triplesMap.TableName != null)
                return string.Format("SELECT * FROM {0}", triplesMap.TableName);

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
                    refObjectMap.JoinConditions.Select(join => string.Format("child.{0}=parent.{1}", join.ChildColumn, join.ParentColumn));

                return string.Format(@"SELECT * FROM ({0}) AS child, 
({1}) AS parent
WHERE {2}", refObjectMap.ChildEffectiveSqlQuery, refObjectMap.ParentEffectiveSqlQuery, string.Join("\nAND ", joinStatements));
            }

            return string.Format("SELECT * FROM ({0}) AS tmp", refObjectMap.ChildEffectiveSqlQuery);
        }

        #endregion
    }
}