using System.IO;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.Log
{
    /// <summary>
    /// Simple implementation of the <see cref="IRDFTermGenerationLog"/> and <see cref="ITriplesGenerationLog"/>
    /// interfaces, which writes messages to a <see cref="TextWriter"/>
    /// todo: refactor to a base class
    /// </summary>
    public class TextWriterLog : IRDFTermGenerationLog, ITriplesGenerationLog
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// Creates an instance <see cref="TextWriterLog"/>, which will write to the given <paramref name="writer"/>
        /// </summary>
        public TextWriterLog(TextWriter writer)
        {
            _writer = writer;
        }

        #region Implementation of IRDFTermGenerationLog
        
        /// <summary>
        /// Logs an error of not found column in the SQL reuslts
        /// </summary>
        public void LogColumnNotFound(ITermMap termMap, string columnName)
        {
            _writer.WriteLine("Column {0} not found", columnName);
        }

        /// <summary>
        /// Logs an RDF term generated
        /// </summary>
        public void LogTermGenerated(INode node)
        {
            _writer.WriteLine("Generated term {0}", node);
        }

        /// <summary>
        /// Logs a null RDF term generated
        /// </summary>
        public void LogNullTermGenerated(ITermMap termMap)
        {
            _writer.WriteLine("Term map {0} produced null term", termMap.Node);
        }

        /// <summary>
        /// Logs a null value retrieved for column
        /// </summary>
        public void LogNullValueForColumn(string columnName)
        {
        }

        #endregion

        #region Implementation of ITriplesGenerationLog

        /// <summary>
        /// Logs an error of missing <see cref="ITriplesMap"/>'s <see cref="ITriplesMap.SubjectMap"/>
        /// </summary>
        public void LogMissingSubject(ITriplesMap triplesMap)
        {
            _writer.WriteLine("Triples map {0} has no subject map", triplesMap.Node);
        }

        /// <summary>
        /// Logs an error in executing an SQL query
        /// </summary>
        public void LogQueryExecutionError(IQueryMap map, string errorMessage)
        {
            _writer.WriteLine("Could not execute query for {0}: {1}", map.Node, errorMessage);
        }

        /// <summary>
        /// Logs an invalid <a href="http://www.w3.org/TR/r2rml/#term-map">term map</a>
        /// </summary>
        public void LogInvalidTermMap(ITermMap termMap, string message)
        {
            _writer.WriteLine("Term map {0} was invalid: {1}", termMap.Node, message);
        }

        /// <summary>
        /// Logs an invalid <a href="http://www.w3.org/TR/r2rml/#triples-map">triples map</a>
        /// </summary>
        public void LogInvaldTriplesMap(ITriplesMap triplesMap, string message)
        {
            _writer.WriteLine("Triples map {0} was invalid: {1}", triplesMap.Node, message);
        }

        #endregion
    }
}