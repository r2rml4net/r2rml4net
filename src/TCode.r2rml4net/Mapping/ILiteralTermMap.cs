using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// A literal term map
    /// </summary>
    public interface ILiteralTermMap : ITermMap
    {
        /// <summary>
        /// Gets the datatype URI of the RDF term generated from this term map
        /// </summary>
        Uri DataTypeURI { get; }
        /// <summary>
        /// Gets the language tag of the RDF term generated from this term map
        /// </summary>
        string Language { get; }
        /// <summary>
        /// Gets the literal value of the RDF term generated from this term map
        /// </summary>
        string Literal { get; }
    }
}