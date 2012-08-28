using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.Log
{
    /// <summary>
    /// Interface for abstract logging of the RDF term generation process
    /// </summary>
    public interface IRDFTermGenerationLog
    {
        /// <summary>
        /// Logs a column missing in the SQL reuslts
        /// </summary>
        void LogColumnNotFound(ITermMap termMap, string columnName);
        /// <summary>
        /// Logs an RDF term generated
        /// </summary>
        void LogTermGenerated(INode node);
        /// <summary>
        /// Logs a null RDF term generated
        /// </summary>
        void LogNullTermGenerated(ITermMap termMap);
        /// <summary>
        /// Logs a null value retrieved for column
        /// </summary>
        void LogNullValueForColumn(string columnName);
    }
}