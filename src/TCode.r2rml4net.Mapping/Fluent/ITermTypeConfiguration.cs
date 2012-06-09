namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Interface for setting term map's term type. More info on http://www.w3.org/TR/r2rml/#dfn-term-type
    /// </summary>
    public interface ITermTypeConfiguration
    {
        /// <summary>
        /// Sets Term Map's term type to blank node. Throws an exception if term map is a graph map or a predicate map
        /// </summary>
        ITermMapConfiguration IsBlankNode();

        /// <summary>
        /// Sets Term Map's term type to blank node. Throws an exception if term map is a graph map or a predicate map
        /// </summary>
        ITermMapConfiguration IsIRI();

        /// <summary>
        /// Sets Term Map's term type to blank node. Throws an exception if term map is not an object map
        /// </summary>
        ITermMapConfiguration IsLiteral();
    }
}