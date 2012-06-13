using System;

namespace TCode.r2rml4net.RDF
{
    public interface IObjectMap : ITermMap
    {
        /// <summary>
        /// Gets constant object URI or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        Uri Object { get; }
        /// <summary>
        /// Gets constant object literal value or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string Literal { get; }
        /// <summary>
        /// Gets column name or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string ColumnName { get; }
    }
}