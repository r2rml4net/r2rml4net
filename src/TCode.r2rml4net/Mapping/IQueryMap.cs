namespace TCode.r2rml4net.Mapping
{
    public interface IQueryMap : IMapBase
    {
        /// <summary>
        /// Gets the effective sql query of a <see cref="ITriplesMap"/> or a <see cref="IRefObjectMap"/>
        /// </summary>
        string EffectiveSqlQuery { get; }
    }
}