namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Marker interface to allow only <see cref="ISubjectMapConfiguration"/> and <see cref="IPredicateObjectMapConfiguration"/>
    /// as parent for <see cref="GraphMapConfiguration"/>
    /// </summary>
    public interface IGraphMapParent : IMapBase
    {
        /// <summary>
        /// Creates an attached <see cref="IGraphMap"/>
        /// </summary>
        IGraphMap CreateGraphMap();
    }
}