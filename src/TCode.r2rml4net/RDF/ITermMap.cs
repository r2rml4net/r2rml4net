using System;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Provides read-only access to term maps
    /// </summary>
    public interface ITermMap
    {
        /// <summary>
        /// Gets column name or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string ColumnName { get; }
        /// <summary>
        /// Gets template or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        string Template { get; }
        /// <summary>
        /// Gets URI constant value or null if absent
        /// </summary>
        /// <exception cref="InvalidTriplesMapException"></exception>
        Uri ConstantValue { get; }
    }
}