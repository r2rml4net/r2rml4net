namespace TCode.r2rml4net.Mapping
{
    public interface ITermType
    {
        bool IsURI { get; }
        bool IsBlankNode { get; }
        bool IsLiteral { get; }
    }
}