namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a map, which provides a SQL query
    /// </summary>
    public interface IQueryMap : IMapBase
    {
        /// <summary>
        /// Gets the effective sql query of a <see cref="ITriplesMap"/> or a <see cref="IRefObjectMap"/>
        /// </summary>
        string EffectiveSqlQuery { get; }
    }
}