namespace TCode.r2rml4net.Mapping.Fluent
{
    interface IObjectMapConfiguration : ITermMapConfiguration
    {
        IObjectMapConfiguration IsConstantValued(string literal);
    }
}