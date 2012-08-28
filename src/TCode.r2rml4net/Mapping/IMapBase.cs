using System;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
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
        /// <summary>
        /// Base mapping URI. It will be used to resolve relative values when generating terms
        /// </summary>
        Uri BaseURI { get; }
    }
}