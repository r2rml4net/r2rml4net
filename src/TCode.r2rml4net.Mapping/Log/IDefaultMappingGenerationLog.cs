using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Log
{
    /// <summary>
    /// Interface for classes implementing logging of the defaul mapping generation process
    /// </summary>
    public interface IDefaultMappingGenerationLog
    {
        /// <summary>
        /// Logs the presence of multiple referenced composite keys. Such mapping should result in the use of all columns
        /// </summary>
        void LogMultipleCompositeKeyReferences(TableMetadata table);
    }
}