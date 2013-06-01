#region Licence
// Copyright (C) 2013 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Configuration;
using VDS.RDF.Nodes;

namespace TCode.r2rml4net.Configuration
{
    public class R2RMLObjectFactory : IObjectFactory
    {
        private static readonly IEnumerable<Type> TypeSupportedExternally;

        static R2RMLObjectFactory()
        {
            TypeSupportedExternally = new[]
                {
                    typeof (W3CR2RMLProcessor),
                    typeof (DirectR2RMLMapping)
                };
        }

        public bool TryLoadObject(IGraph configGraph, INode objNode, Type targetType, out object loadedObject)
        {
            Debug.WriteLine("Loading {0}", targetType);

            try
            {
                if (targetType == typeof(W3CR2RMLProcessor))
                {
                    string connectionType =
                        configGraph.GetTriplesWithSubjectPredicate(objNode,
                                                                   configGraph.CreateUriNode(
                                                                       UriFactory.Create(
                                                                           "http://r2rml.net/configuration#connectionType")))
                                   .Single()
                                   .Object.ToString();


                    IDbConnection connection =
                        (IDbConnection)Activator.CreateInstance(Type.GetType(connectionType, true));
                    connection.ConnectionString = configGraph.GetTriplesWithSubjectPredicate(objNode,
                                                                                             configGraph.CreateUriNode(
                                                                                                 UriFactory.Create(
                                                                                                     "http://r2rml.net/configuration#connectionString")))
                                                             .Single()
                                                             .Object.ToString();

                    var options = configGraph.GetTriplesWithSubjectPredicate(objNode,
                                                                             configGraph.CreateUriNode(
                                                                                 UriFactory.Create(
                                                                                     "http://r2rml.net/configuration#mappingOptions")))
                                             .SingleOrDefault();

                    if (options != null)
                    {
                        object mappingOptions;
                        if (TryLoadObject(configGraph, options.Object, typeof(MappingOptions), out mappingOptions))
                        {
                            loadedObject = new W3CR2RMLProcessor(connection, (MappingOptions)mappingOptions);
                        }
                        else
                        {
                            Debug.WriteLine("Failed loading MappingOptions");
                            loadedObject = null;
                        }
                    }
                    else
                    {
                        loadedObject = new W3CR2RMLProcessor(connection);
                    }
                }
                else if (targetType == typeof(MappingOptions))
                {
                    var mappingOptions = new MappingOptions();

                    foreach (var triple in configGraph.GetTriplesWithSubject(objNode))
                    {
                        switch (((IUriNode)triple.Predicate).Uri.ToString())
                        {
                            case "http://r2rml.net/configuration#blankNodeTemplateSeparator":
                                mappingOptions.BlankNodeTemplateSeparator = triple.Object.AsValuedNode().AsString();
                                break;
                            case "http://r2rml.net/configuration#useDelimitedIdentifiers":
                                mappingOptions.UseDelimitedIdentifiers = triple.Object.AsValuedNode().AsBoolean();
                                break;
                            case "http://r2rml.net/configuration#sqlIdentifierDelimiter":
                                char delimiter = triple.Object.AsValuedNode().AsString().First();
                                mappingOptions.SetSqlIdentifierDelimiters(delimiter, delimiter);
                                break;
                            case "http://r2rml.net/configuration#validateSqlVersion":
                                mappingOptions.ValidateSqlVersion = triple.Object.AsValuedNode().AsBoolean();
                                break;
                            case "http://r2rml.net/configuration#ignoreMappingErrors":
                                mappingOptions.IgnoreMappingErrors = triple.Object.AsValuedNode().AsBoolean();
                                break;
                            case "http://r2rml.net/configuration#ignoreDataErrors":
                                mappingOptions.IgnoreDataErrors = triple.Object.AsValuedNode().AsBoolean();
                                break;
                            case "http://r2rml.net/configuration#preserveDuplicateRows":
                                mappingOptions.PreserveDuplicateRows = triple.Object.AsValuedNode().AsBoolean();
                                break;
                        }
                    }

                    loadedObject = mappingOptions;
                }
                else
                {
                    loadedObject = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                loadedObject = null;
                return false;
            }

            return loadedObject != null;
        }

        public bool CanLoadObject(Type typeToLoad)
        {
            return TypeSupportedExternally.Contains(typeToLoad);
        }
    }
}