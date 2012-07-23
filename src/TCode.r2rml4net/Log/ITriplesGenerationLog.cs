using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Log
{
    /// <summary>
    /// Interface for abstract logging of the triples generation process
    /// </summary>
    public interface ITriplesGenerationLog
    {
        /// <summary>
        /// Logs an error of missing <see cref="ITriplesMap"/>'s <see cref="ITriplesMap.SubjectMap"/>
        /// </summary>
        void LogMissingSubject(ITriplesMap triplesMap);
    }
}