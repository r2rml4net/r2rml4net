﻿using TCode.r2rml4net.Mapping;

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