namespace TCode.r2rml4net.Mapping.Fluent
{
    public interface IObjectMapConfiguration : ITermMapConfiguration
    {
        ILiteralTermMapConfiguration IsConstantValued(string literal);
        ILiteralTermMapConfiguration IsColumnValued(string columnName);
    }
}