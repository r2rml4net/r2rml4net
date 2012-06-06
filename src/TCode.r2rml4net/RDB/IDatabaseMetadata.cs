namespace TCode.r2rml4net.RDB
{
    public interface IDatabaseMetadata
    {
        void ReadMetadata();
        TableCollection Tables { get; }
    }
}
