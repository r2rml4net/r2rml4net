#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
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
using System.Linq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net
{
    /// <summary>
    /// Entrypoint to fluent configuration of R2RML, backed by DotNetRDF
    /// </summary>
    public class FluentR2RML : BaseConfiguration, IR2RMLConfiguration
    {
        internal static Uri DefaultBaseUri
        {
            get
            {
                return new Uri("http://r2rml.net/mappings/");
            }
        }

        IGraph _graphCopy;
        private ISqlVersionValidator _sqlVersionValidator = new Wc3SqlVersionValidator();
        ISqlQueryBuilder _sqlQueryBuilder = new W3CSqlQueryBuilder();

        readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings
        /// </summary>
        /// <param name="baseUri">base URI for mapping nodes</param>
        public FluentR2RML(Uri baseUri)
            : base(baseUri, new MappingOptions())
        {
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings
        /// </summary>
        /// <param name="baseUri">base URI for mapping nodes</param>
        /// <param name="mappingOptions"><see cref="MappingOptions"/></param>
        public FluentR2RML(Uri baseUri, MappingOptions mappingOptions)
            : base(baseUri, mappingOptions)
        {
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        /// <summary>
        /// Creates a new instance of R2RMLConfiguration with empty R2RML mappings 
        /// and base URI set to <see cref="DefaultBaseUri"/>
        /// </summary>
        public FluentR2RML()
            : this(new MappingOptions())
        {
        }

        /// <param name="mappingOptions"><see cref="MappingOptions"/></param>
        public FluentR2RML(MappingOptions mappingOptions)
            : base(DefaultBaseUri, mappingOptions)
        {

        }

        internal FluentR2RML(IGraph mappingsGraph, MappingOptions mappingOptions)
            : base(mappingsGraph, mappingOptions)
        {

        }

        #region Overrides of BaseConfiguration

        /// <summary>
        /// Creates triples maps configuration objects for the current mapping file
        /// </summary>
        /// <remarks>Used in loading configuration from an exinsting graph</remarks>
        protected override void InitializeSubMapsFromCurrentGraph()
        {
            if (R2RMLMappings == null)
                return;

            var rdfType = R2RMLMappings.CreateUriNode(R2RMLUris.RdfType);
            var triplesMapClass = R2RMLMappings.CreateUriNode(R2RMLUris.RrTriplesMapClass);
            var triplesMapsTriples = R2RMLMappings.GetTriplesWithPredicateObject(rdfType, triplesMapClass).ToArray();
            IDictionary<INode, TriplesMapConfiguration> triplesMaps = new Dictionary<INode, TriplesMapConfiguration>();

            foreach (var triplesMapNode in triplesMapsTriples.Select(triple => triple.Subject))
            {
                var triplesMapConfiguration = new TriplesMapConfiguration(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), triplesMapNode);
                triplesMaps.Add(triplesMapNode, triplesMapConfiguration);
                _triplesMaps.Add(triplesMapConfiguration);
            }

            foreach (var triplesMapPair in triplesMaps)
            {
                triplesMapPair.Value.RecursiveInitializeSubMapsFromCurrentGraph();
            }
        }

        /// <summary>
        /// Always returns false
        /// </summary>
        protected override bool UsesNode
        {
            get
            {
                return false;
            }
        }

        #endregion

        void R2RMLMappingsChanged(object sender, GraphEventArgs args)
        {
            _graphCopy = null;
        }

        /// <summary>
        /// Creates a Triples Map with physical table datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromTable(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), tablename));
        }

        /// <summary>
        /// Creates a Triples Map with R2RML view datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromSqlQuery(new TriplesMapConfigurationStub(this, R2RMLMappings, MappingOptions, SqlVersionValidator), sqlQuery));
        }

        private TriplesMapConfiguration SetupTriplesMap(TriplesMapConfiguration triplesMapConfiguration)
        {
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }

        /// <summary>
        /// Base mapping URI. It will be used to resolve relative values when generating terms
        /// </summary>
        public Uri BaseUri
        {
            get { return R2RMLMappings.BaseUri; }
        }

        /// <summary>
        /// Returns copy of the mapping graph
        /// </summary>
        public IGraph GraphReadOnly
        {
            get
            {
                if (_graphCopy == null)
                {
                    _graphCopy = new Graph(R2RMLMappings.Triples);
                    _graphCopy.NamespaceMap.Import(R2RMLMappings.NamespaceMap);
                    _graphCopy.BaseUri = R2RMLMappings.BaseUri;
                }

                return _graphCopy;
            }
        }

        #region Implementation of IR2RML

        /// <summary>
        /// Gets or sets the <see cref="ISqlQueryBuilder"/>
        /// </summary>
        public ISqlQueryBuilder SqlQueryBuilder
        {
            get { return _sqlQueryBuilder; }
            set { _sqlQueryBuilder = value; }
        }

        /// <summary>
        /// <see cref="ISqlVersionValidator"/>
        /// </summary>
        public ISqlVersionValidator SqlVersionValidator
        {
            get { return _sqlVersionValidator; }
            set { _sqlVersionValidator = value; }
        }

        /// <summary>
        /// <see cref="IR2RML.TriplesMaps"/>
        /// </summary>
        public IEnumerable<ITriplesMap> TriplesMaps
        {
            get { return _triplesMaps; }
        }

        #endregion
    }
}