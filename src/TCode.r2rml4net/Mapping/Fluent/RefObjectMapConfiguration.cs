#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
using TCode.r2rml4net.Extensions;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent
{
    internal class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration
    {
        private readonly ITriplesMap _parentTriplesMap;
        private readonly IPredicateObjectMap _predicateObjectMap;
        private readonly ITriplesMapConfiguration _childTriplesMap;

        internal RefObjectMapConfiguration(
            IPredicateObjectMap predicateObjectMap, 
            ITriplesMapConfiguration childTriplesMap, 
            ITriplesMap parentTriplesMap, 
            IGraph mappings)
            : this(predicateObjectMap, childTriplesMap, parentTriplesMap, mappings, mappings.CreateBlankNode())
        {
        }

        internal RefObjectMapConfiguration(
            IPredicateObjectMap predicateObjectMap,
            ITriplesMapConfiguration childTriplesMap,
            ITriplesMap parentTriplesMap,
            IGraph mappings,
            INode node)
            : base(childTriplesMap, mappings, node)
        {
            _predicateObjectMap = predicateObjectMap;
            _childTriplesMap = childTriplesMap;
            _parentTriplesMap = parentTriplesMap;

            AssertObjectMapSubgraph();
        }

        // todo: this is for ignored tests?
        ////protected internal override void RecursiveInitializeSubMapsFromCurrentGraph()
        ////{
        ////    R2RMLMappings.Retract(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), Node);
        ////    R2RMLMappings.Retract(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);

        ////    Node = refObjectMapNode;
        ////    AssertObjectMapSubgraph();

        ////    base.RecursiveInitializeSubMapsFromCurrentGraph();
        ////}

        public IEnumerable<JoinCondition> JoinConditions
        {
            get
            {
                var joinConditions = Node.GetObjects(R2RMLUris.RrJoinCondition);

                foreach (var joinCondition in joinConditions)
                {
                    var child = joinCondition.GetObjects(R2RMLUris.RrChild).GetSingleOrDefault().GetLiteral();
                    var parent = joinCondition.GetObjects(R2RMLUris.RrParent).GetSingleOrDefault().GetLiteral();

                    yield return new JoinCondition(child, parent);
                }
            }
        }

        public string ChildEffectiveSqlQuery
        {
            get { return _childTriplesMap.EffectiveSqlQuery; }
        }

        public string ParentEffectiveSqlQuery
        {
            get { return _parentTriplesMap.EffectiveSqlQuery; }
        }

        public ISubjectMap SubjectMap
        {
            get { return _parentTriplesMap.SubjectMap; }
        }

        public IPredicateObjectMap PredicateObjectMap
        {
            get { return _predicateObjectMap; }
        }

        public string EffectiveSqlQuery
        {
            get
            {
                return TriplesMap.R2RMLConfiguration.SqlQueryBuilder.GetEffectiveQueryForRefObjectMap(this);
            }
        }

        public ITriplesMap ParentTriplesMap
        {
            get
            {
                return _parentTriplesMap;
            }
        }

        public void AddJoinCondition(string childColumn, string parentColumn)
        {
            IBlankNode joinConditionNode = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrJoinCondition), joinConditionNode);
            R2RMLMappings.Assert(joinConditionNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrChild), R2RMLMappings.CreateLiteralNode(childColumn));
            R2RMLMappings.Assert(joinConditionNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrParent), R2RMLMappings.CreateLiteralNode(parentColumn));
        }

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            AssertObjectMapSubgraph();
        }

        private void AssertObjectMapSubgraph()
        {
            R2RMLMappings.Assert(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), Node);
            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);
        }
    }
}
