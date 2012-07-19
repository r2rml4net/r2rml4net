using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Contract for classes building effective sql queries for <see cref="ITriplesMap"/> and <see cref="IRefObjectMap"/>
    /// </summary>
    public interface IEffectiveSqlBuilder
    {
        /// <summary>
        /// Gets effective sql query based on <see cref="ITriplesMap.TableName"/> or <see cref="ITriplesMap.SqlQuery"/>
        /// </summary>
        string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap);
        /// <summary>
        /// Gets effective sql query based on parent/child <see cref="ITriplesMap.EffectiveSqlQuery"/> maps and <see cref="IRefObjectMap.JoinConditions"/> (if any)
        /// </summary>
        string GetEffectiveQueryForRefObjectMap(IRefObjectMap refObjectMap);
    }
}