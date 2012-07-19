using System.Linq;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.RDB
{
    public class W3CEffectiveSqlBuilder : IEffectiveSqlBuilder
    {
        #region Implementation of IEffectiveSqlBuilder

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