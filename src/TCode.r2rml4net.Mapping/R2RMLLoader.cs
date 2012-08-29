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

using System.IO;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Class for loading existing R2RML mappings
    /// </summary>
    public static class R2RMLLoader
    {
        /// <summary>
        /// Loads R2RML mappings from a string
        /// </summary>
        public static IR2RML Load(string r2RMLGraph)
        {
            return Load(r2RMLGraph, new MappingOptions());
        }

        /// <summary>
        /// Loads R2RML mappings from a stream
        /// </summary>
        public static IR2RML Load(Stream r2RMLGraph)
        {
            return Load(r2RMLGraph, new MappingOptions());
        }
        /// <summary>
        /// Loads R2RML mappings from a string
        /// </summary>
        public static IR2RML Load(string r2RMLGraph, MappingOptions mappingOptions)
        {
            IGraph graph = new Graph();
            graph.LoadFromString(r2RMLGraph);

            return InitializeMappings(graph, mappingOptions);
        }

        /// <summary>
        /// Loads R2RML mappings from a stream
        /// </summary>
        public static IR2RML Load(Stream r2RMLGraph, MappingOptions mappingOptions)
        {
            using (var reader = new StreamReader(r2RMLGraph))
            {
                return Load(reader.ReadToEnd(), mappingOptions);
            }
        }

        private static IR2RML InitializeMappings(IGraph graph, MappingOptions mappingOptions)
        {
            var mappings = new R2RMLConfiguration(graph, mappingOptions);
            mappings.RecursiveInitializeSubMapsFromCurrentGraph();
            return mappings;
        }
    }
}
