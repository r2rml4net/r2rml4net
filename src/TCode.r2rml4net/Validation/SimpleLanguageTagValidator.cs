#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

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
        public virtual bool LanguageTagIsValid(string languageTag)
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