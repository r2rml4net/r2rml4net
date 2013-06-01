#region Licence
			
/* 
Copyright (C) 2013 Tomasz Pluskiewicz
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace TCode.r2rml4net.Configuration
{
    internal class MappingOptionsLoader
    {
        private static readonly IDictionary<string, Action<MappingOptions, IValuedNode>> PropertySetters;

        static MappingOptionsLoader()
        {
            PropertySetters = new Dictionary<string, Action<MappingOptions, IValuedNode>>
                {
                    {Ontology.UseDelimitedIdentifiers, SetUseDelimitedIdentifiers},
                    {Ontology.BlankNodeTemplateSeparator, SetBNodeSeparator},
                    {Ontology.SqlIdentifierDelimiter, SetSqlIdentifierDelimiter},
                    {Ontology.ValidateSqlVersion, SetValidateSqlVersion},
                    {Ontology.IgnoreMappingErrors, SetIgnoreMappingErrors},
                    {Ontology.IgnoreDataErrors, SetIgnoreDataErrors},
                    {Ontology.PreserveDuplicateRows, SetPreserveDuplicateRows}
                };
        }

        public MappingOptions Load(IGraph configGraph, INode mappingOptionsNode)
        {
            Debug.WriteLine("Loading {0} from node {1}", typeof(MappingOptions), mappingOptionsNode);

            var mappingOptions = new MappingOptions();

            foreach (var triple in configGraph.GetTriplesWithSubject(mappingOptionsNode))
            {
                var predicate = (IUriNode)triple.Predicate;
                var objectNode = triple.Object.AsValuedNode();

                if (PropertySetters.ContainsKey(predicate.Uri.ToString()))
                {
                    PropertySetters[predicate.Uri.ToString()](mappingOptions, objectNode);
                }
                else
                {
                    Debug.WriteLine("Unrecognized property {0}", predicate.Uri);
                }
            }

            return mappingOptions;
        }

        private static void SetPreserveDuplicateRows(MappingOptions options, IValuedNode node)
        {
            options.PreserveDuplicateRows = node.AsBoolean();
        }

        private static void SetIgnoreDataErrors(MappingOptions options, IValuedNode node)
        {
            options.IgnoreDataErrors = node.AsBoolean();
        }

        private static void SetIgnoreMappingErrors(MappingOptions options, IValuedNode node)
        {
            options.IgnoreMappingErrors = node.AsBoolean();
        }

        private static void SetValidateSqlVersion(MappingOptions options, IValuedNode node)
        {
            options.ValidateSqlVersion = node.AsBoolean();
        }

        private static void SetSqlIdentifierDelimiter(MappingOptions options, IValuedNode node)
        {
            var delimiter = node.AsString().First();
            options.SetSqlIdentifierDelimiters(delimiter, delimiter);
        }

        private static void SetUseDelimitedIdentifiers(MappingOptions options, IValuedNode node)
        {
            options.UseDelimitedIdentifiers = node.AsBoolean();
        }

        private static void SetBNodeSeparator(MappingOptions options, IValuedNode node)
        {
            options.BlankNodeTemplateSeparator = node.AsString();
        }
    }
}