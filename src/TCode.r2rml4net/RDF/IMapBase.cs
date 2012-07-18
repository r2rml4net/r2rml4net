using VDS.RDF;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// A base interface for all maps which are represented by a node in RDF
    /// </summary>
    public interface IMapBase
    {
        /// <summary>
        /// The node representing this <see cref="IMapBase"/>
        /// </summary>
        INode Node { get; }
    }
}