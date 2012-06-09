using System;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Interface for creating configuration of term maps as described on http://www.w3.org/TR/r2rml/#term-map
    /// </summary>
    public interface ITermMapConfiguration
    {
        /// <summary>
        /// Returns term map's term type set with <see cref="TermType"/> method 
        /// or a default value as described on http://www.w3.org/TR/r2rml/#dfn-term-type 
        /// </summary>
        Uri TermTypeIRI { get; }

        /// <summary>
        /// Allows setting the term type of the current term map
        /// </summary>
        /// <remarks>Please see <see cref="ITermTypeConfiguration"/> members for info on valid values for different term maps</remarks>
        ITermTypeConfiguration TermType();
    }
}
