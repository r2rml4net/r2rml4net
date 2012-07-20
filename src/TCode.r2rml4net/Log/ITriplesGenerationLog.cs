using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Log
{
    public interface ITriplesGenerationLog
    {
        void LogMissingSubject(ITriplesMap triplesMap);
    }
}