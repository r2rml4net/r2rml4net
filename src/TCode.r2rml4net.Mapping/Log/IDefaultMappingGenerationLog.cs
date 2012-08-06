using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Log
{
    public interface IDefaultMappingGenerationLog
    {
        void LogMultipleCompositeKeyReferences(TableMetadata table);
    }
}