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
        /// <summary>
        /// Logs an error in executing an SQL query
        /// </summary>
        void LogQueryExecutionError(IQueryMap map, string errorMessage);
        /// <summary>
        /// Logs an invalid <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
        /// </summary>
        void LogInvalidTermMap(ITermMap termMap, string message);
        /// <summary>
        /// Logs an invalid <a href="http://www.w3.org/TR/r2rml/#triples-map">triples map</a>
        /// </summary>
        void LogInvaldTriplesMap(ITriplesMap triplesMap, string message);
    }
}