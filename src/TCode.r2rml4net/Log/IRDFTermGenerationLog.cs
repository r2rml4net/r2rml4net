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
        /// Logs an error of not found column in the SQL reuslts
        /// </summary>
        void LogColumnNotFound(ITermMap termMap, string columnName);

        void LogTermGenerated(INode node);
        void LogNullTermGenerated(ITermMap termMap);
    }
}