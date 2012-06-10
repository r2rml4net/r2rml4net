namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface IPropertyObjectMapConfiguration
    {
        ITermMapConfiguration CreateObjectMap();
        ITermMapConfiguration CreatePropertyMap();
    }
}