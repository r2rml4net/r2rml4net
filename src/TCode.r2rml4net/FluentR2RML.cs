#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
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
using System.Collections.Generic;
using System.Linq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net
{
    /// <summary>
    /// Entrypoint to fluent configuration of R2RML, backed by DotNetRDF
    /// </summary>
    public class FluentR2RML : BaseConfiguration, IR2RMLConfiguration
    {
        private readonly IList<ITriplesMapConfiguration> _triplesMaps = new List<ITriplesMapConfiguration>();
        private IGraph _graphCopy;
        private ISqlVersionValidator _sqlVersionValidator = new Wc3SqlVersionValidator();
        private ISqlQueryBuilder _sqlQueryBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentR2RML"/> class.
        /// </summary>
        public FluentR2RML(MappingOptions options)
            : base(options.BaseUri)
        {
            Options = options;
            this._sqlQueryBuilder = new W3CSqlQueryBuilder(options);
            R2RMLMappings.Changed += R2RMLMappingsChanged;
        }

        internal FluentR2RML(IGraph mappingsGraph, MappingOptions options)
            : base(mappingsGraph)
        {
            this.Options = options;
            this._sqlQueryBuilder = new W3CSqlQueryBuilder(options);
        }

        public MappingOptions Options { get; }

        /// <summary>
        /// Gets a copy of the mapping graph
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

        public IGraph MappingsGraph => this.GraphReadOnly;

        /// <inheritdoc />
        public ISqlQueryBuilder SqlQueryBuilder
        {
            get { return _sqlQueryBuilder; }
            set { _sqlQueryBuilder = value; }
        }

        /// <inheritdoc />
        public ISqlVersionValidator SqlVersionValidator
        {
            get { return _sqlVersionValidator; }
            set { _sqlVersionValidator = value; }
        }

        /// <inheritdoc />
        public IEnumerable<ITriplesMap> TriplesMaps
        {
            get { return _triplesMaps; }
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

        /// <summary>
        /// Creates a Triples Map with physical table datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapConfiguration CreateTriplesMapFromTable(string tablename)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromTable(new TriplesMapConfigurationStub(this, R2RMLMappings, SqlVersionValidator), tablename, this.Options));
        }

        /// <summary>
        /// Creates a Triples Map with R2RML view datasource and adds it to the R2RML mappings
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration CreateTriplesMapFromR2RMLView(string sqlQuery)
        {
            return SetupTriplesMap(TriplesMapConfiguration.FromSqlQuery(new TriplesMapConfigurationStub(this, R2RMLMappings, SqlVersionValidator), sqlQuery, this.Options));
        }

        /// <summary>
        /// Creates triples maps configuration objects for the current mapping file
        /// </summary>
        /// <remarks>Used in loading configuration from an exinsting graph</remarks>
        protected override void InitializeSubMapsFromCurrentGraph()
        {
            if (R2RMLMappings == null)
            {
                return;
            }

            var subjectMapProperty = R2RMLMappings.CreateUriNode(R2RMLUris.RrSubjectMapProperty);
            var triplesMapsTriples = R2RMLMappings.GetTriplesWithPredicate(subjectMapProperty).ToArray();
            IDictionary<INode, TriplesMapConfiguration> triplesMaps = new Dictionary<INode, TriplesMapConfiguration>();

            foreach (var triplesMapNode in triplesMapsTriples.Select(triple => triple.Subject))
            {
                var triplesMapConfiguration = new TriplesMapConfiguration(new TriplesMapConfigurationStub(this, R2RMLMappings, SqlVersionValidator), triplesMapNode, this.Options);
                triplesMaps.Add(triplesMapNode, triplesMapConfiguration);
                _triplesMaps.Add(triplesMapConfiguration);
            }

            foreach (var triplesMapPair in triplesMaps)
            {
                triplesMapPair.Value.RecursiveInitializeSubMapsFromCurrentGraph();
            }
        }

        private void R2RMLMappingsChanged(object sender, GraphEventArgs args)
        {
            _graphCopy = null;
        }

        private TriplesMapConfiguration SetupTriplesMap(TriplesMapConfiguration triplesMapConfiguration)
        {
            _triplesMaps.Add(triplesMapConfiguration);
            return triplesMapConfiguration;
        }
    }
}