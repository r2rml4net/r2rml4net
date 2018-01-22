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
using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using TCode.r2rml4net.Extensions;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Fluent configuration of subject map backed by a DotNetRDF graph (see <see cref="ISubjectMapConfiguration"/>)
    /// </summary>
    [NullGuard(ValidationFlags.All)]
    internal class SubjectMapConfiguration : TermMapConfiguration, ISubjectMapConfiguration, INonLiteralTermMapConfigutarion
    {
        private readonly IList<GraphMapConfiguration> _graphMaps = new List<GraphMapConfiguration>();

        internal SubjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraph r2RMLMappings)
            : this(parentTriplesMap, r2RMLMappings, r2RMLMappings.CreateBlankNode())
        {
        }

        internal SubjectMapConfiguration(ITriplesMapConfiguration parentTriplesMap, IGraph r2RMLMappings, INode node)
            : base(parentTriplesMap, parentTriplesMap, r2RMLMappings, node)
        {
        }

        /// <inheritdoc/>
        public Uri[] Classes
        {
            get
            {
                return (from obj in Node.GetObjects(R2RMLUris.RrClassProperty)
                        select obj.GetUri()).ToArray();
            }
        }

        /// <inheritdoc/>
        public Uri URI
        {
            [return: AllowNull]
            get { return ConstantValue; }
        }

        /// <inheritdoc/>
        public IEnumerable<IGraphMap> GraphMaps
        {
            get { return _graphMaps; }
        }

        /// <inheritdoc/>
        public IGraphMapConfiguration CreateGraphMap()
        {
            var graphMap = new GraphMapConfiguration(TriplesMap, this, R2RMLMappings);
            _graphMaps.Add(graphMap);
            return graphMap;
        }

        /// <summary>
        /// <see cref="ISubjectMapConfiguration.AddClass"/>
        /// </summary>
        public ISubjectMapConfiguration AddClass(Uri classIri)
        {
            // create SubjectMap - TriplesMap relation if no class has been added
            if (Classes.Length == 0)
            {
                CreateParentMapRelation();
            }

            R2RMLMappings.Assert(
                Node,
                R2RMLMappings.CreateUriNode(R2RMLUris.RrClassProperty),
                R2RMLMappings.CreateUriNode(classIri));

            return this;
        }

        /// <summary>
        /// Returns rr:subjectMap
        /// </summary>
        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrSubjectMapProperty);
        }

        protected internal override IUriNode CreateShortcutPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(R2RMLUris.RrSubjectProperty);
        }

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            CreateSubMaps(R2RMLUris.RrGraphMapPropety, (graph, node) => new GraphMapConfiguration(TriplesMap, this, graph, node), _graphMaps);
        }
    }
}