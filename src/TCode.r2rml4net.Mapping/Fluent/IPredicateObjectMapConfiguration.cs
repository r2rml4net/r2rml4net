namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface IPredicateObjectMapConfiguration
    {
        ITermMapConfiguration CreateObjectMap();
        ITermMapConfiguration CreatePredicateMap();
    }
}