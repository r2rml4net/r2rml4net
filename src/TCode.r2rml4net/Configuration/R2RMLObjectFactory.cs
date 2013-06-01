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
using VDS.RDF;
using VDS.RDF.Configuration;
using VDS.RDF.Nodes;

namespace TCode.r2rml4net.Configuration
{
    public class R2RMLObjectFactory : IObjectFactory
    {
        private static readonly IDictionary<Type, Func<IGraph, INode, object>> TypeLoadMethods;

        private static readonly MappingOptionsLoader MappingOptionsLoader;

        static R2RMLObjectFactory()
        {
            TypeLoadMethods = new Dictionary<Type, Func<IGraph, INode, object>>
                {
                    {typeof (W3CR2RMLProcessor), LoadProcessor},
                    {typeof (DirectR2RMLMapping), LoadDirectMapping}
                };
            MappingOptionsLoader = new MappingOptionsLoader();
        }

        public bool TryLoadObject(IGraph configGraph, INode objNode, Type targetType, out object loadedObject)
        {
            if (!CanLoadObject(targetType))
            {
                throw new ArgumentException(string.Format("Cannot load type {0}", targetType), "targetType");
            }

            Debug.WriteLine("Loading {0} from node {1}", targetType, objNode);

            try
            {
                loadedObject = TypeLoadMethods[targetType](configGraph, objNode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }

            return true;
        }

        private static W3CR2RMLProcessor LoadProcessor(IGraph configGraph, INode objNode)
        {
            var connection = LoadConnection(configGraph, objNode);

            var optionsNode = configGraph.GetSingleOrDefaultTripleObject(objNode, Ontology.MappingOptions);

            if (optionsNode != null)
            {
                var mappingOptions = MappingOptionsLoader.Load(configGraph, optionsNode);
                return new W3CR2RMLProcessor(connection, mappingOptions);
            }
            else
            {
                return new W3CR2RMLProcessor(connection);
            }
        }

        private static DirectR2RMLMapping LoadDirectMapping(IGraph configGraph, INode objNode)
        {
            throw new NotImplementedException();
        }

        private static IDbConnection LoadConnection(IGraph configGraph, INode objNode)
        {
            string connectionType = configGraph.GetSingleTripleObject(objNode, Ontology.ConnectionType)
                                               .AsValuedNode().AsString();

            IDbConnection connection = (IDbConnection)Activator.CreateInstance(Type.GetType(connectionType, true));
            connection.ConnectionString = configGraph.GetSingleTripleObject(objNode, Ontology.ConnectionString)
                                                     .AsValuedNode().AsString();
            return connection;
        }

        public bool CanLoadObject(Type typeToLoad)
        {
            return TypeLoadMethods.ContainsKey(typeToLoad);
        }
    }
}