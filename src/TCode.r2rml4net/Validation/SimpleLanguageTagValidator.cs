using System;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Validation
{
    /// <summary>
    /// A simple implementation, which looks the language tag up
    /// in a graph extracted from <a href="http://www.lexvo.org/">Lexvo.org</a>
    /// </summary>
    /// <remarks>the graph is an embedded turtle file located in TCode.r2rml4net.DataSets.languages.ttl</remarks>
    public class SimpleLanguageTagValidator : ILanguageTagValidator
    {
        private static readonly object ClassLock = new object();
        private static IGraph _languagesGraph;

        private static IGraph LanguagesGraph
        {
            get
            {
                lock (ClassLock)
                {
                    if (_languagesGraph == null)
                    {
                        lock (ClassLock)
                        {
                            _languagesGraph = new Graph();
                            _languagesGraph.LoadFromEmbeddedResource("TCode.r2rml4net.DataSets.languages.ttl, TCode.r2rml4net");
                        }
                    } 
                }

                return _languagesGraph;
            }
        }

        /// <summary>
        /// Check wheather the <paramref name="languageTag"/> is valid
        /// </summary>
        /// <returns>true if language tag is valid</returns>
        public bool LanguageTagIsValid(string languageTag)
        {
            if (languageTag == null) throw new ArgumentNullException("languageTag");
            if (string.IsNullOrWhiteSpace(languageTag)) throw new ArgumentException("languageTag");

            var query = new SparqlParameterizedString(@"ASK WHERE { [] <urn:lang:code> ?code. FILTER(?code = @languageCode) }");
            query.SetLiteral("languageCode", languageTag.Split('-').First());

            var triples = (SparqlResultSet)LanguagesGraph.ExecuteQuery(query);

            return triples.Result;
        }
    }
}