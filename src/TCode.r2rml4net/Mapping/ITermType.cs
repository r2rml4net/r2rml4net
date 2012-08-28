namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Interface for class representing the <a href="http://www.w3.org/TR/r2rml/#termtype">term type</a>
    /// </summary>
    public interface ITermType
    {
        /// <summary>
        /// Gets value indicating whether the term map's term type is rr:IRI
        /// </summary>
        bool IsURI { get; }
        /// <summary>
        /// Gets value indicating whether the term map's term type is rr:BlankNode
        /// </summary>
        bool IsBlankNode { get; }
        /// <summary>
        /// Gets value indicating whether the term map's term type is rr:Literal
        /// </summary>
        bool IsLiteral { get; }
    }
}