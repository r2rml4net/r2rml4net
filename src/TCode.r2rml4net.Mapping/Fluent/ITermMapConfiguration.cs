using System;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Interface for creating configuration of term maps as described on http://www.w3.org/TR/r2rml/#term-map
    /// </summary>
    public interface ITermMapConfiguration
    {
        /// <summary>
        /// Allows setting the term type of the current term map
        /// </summary>
        /// <remarks>Please see <see cref="ITermTypeConfiguration"/> members for info on valid values for different term maps</remarks>
        ITermTypeConfiguration TermType { get; }

        /// <summary>
        /// Sets the term map as constant-valued
        /// </summary>
        /// <param name="uri">Constant value URI</param>
        /// <remarks>
        /// Asserted using the rr:constant property described on http://www.w3.org/TR/r2rml/#constant
        /// </remarks>
        ITermTypeConfiguration IsConstantValued(Uri uri);

        /// <summary>
        /// Sets the term map as template-values
        /// </summary>
        /// <param name="template">A valid template</param>
        /// <remarks>
        /// For details on templates read more on http://www.w3.org/TR/r2rml/#from-template
        /// </remarks>
        ITermTypeConfiguration IsTemplateValued(string template);
    }
}
