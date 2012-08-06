using System.IO;
using TCode.r2rml4net.Mapping;
using VDS.RDF;

namespace TCode.r2rml4net.Log
{
    // todo: refactor to a base class
    public class TextWriterLog : IRDFTermGenerationLog, ITriplesGenerationLog
    {
        private readonly TextWriter _writer;

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

        public void LogTermGenerated(INode node)
        {
            _writer.WriteLine("Generated term {0}", node);
        }

        public void LogNullTermGenerated(ITermMap termMap)
        {
            _writer.WriteLine("Term map {0} produced null term", termMap.Node);
        }

        public void LogInvalidBlankNode(ITermMap termMap, string blankNodeIdentifier)
        {
        }

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

        public void LogQueryExecutionError(IQueryMap map, string errorMessage)
        {
            _writer.WriteLine("Could not execute query for {0}: {1}", map.Node, errorMessage);
        }

        public void LogInvalidTermMap(ITermMap termMap, string message)
        {
            _writer.WriteLine("Term map {0} was invalid: {1}", termMap.Node, message);
        }

        public void LogInvaldTriplesMap(ITriplesMap triplesMap, string message)
        {
            _writer.WriteLine("Triples map {0} was invalid: {1}", triplesMap.Node, message);
        }

        #endregion
    }
}