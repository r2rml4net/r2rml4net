using System;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping.DataSets
{
    internal static class Languages
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
                            _languagesGraph.LoadFromEmbeddedResource("TCode.r2rml4net.Mapping.DataSets.languages.ttl, TCode.r2rml4net.Mapping");
                        }
                    } 
                }

                return _languagesGraph;
            }
        }

        public static bool LanguageTagIsValid(string languageTag)
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