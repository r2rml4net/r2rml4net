using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping
{
    internal class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration
    {
        INode _refObjectMapNode;
        readonly ITriplesMap _parentTriplesMap;
        private readonly IPredicateObjectMap _predicateObjectMap;
        readonly ITriplesMapConfiguration _childTriplesMap;

        internal RefObjectMapConfiguration(IPredicateObjectMap predicateObjectMap, ITriplesMapConfiguration childTriplesMap, ITriplesMap parentTriplesMap, IGraph mappings)
             : base(childTriplesMap, mappings)
        {
            _refObjectMapNode = mappings.CreateBlankNode();
            _predicateObjectMap = predicateObjectMap;
            _childTriplesMap = childTriplesMap;
            _parentTriplesMap = parentTriplesMap;

            AssertObjectMapSubgraph();
        }

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
        }

        protected internal override void RecursiveInitializeSubMapsFromCurrentGraph(INode refObjectMapNode)
        {
            if (refObjectMapNode == null)
                throw new ArgumentNullException("refObjectMapNode");

            R2RMLMappings.Retract(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), _refObjectMapNode);
            R2RMLMappings.Retract(_refObjectMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);

            _refObjectMapNode = refObjectMapNode;
            AssertObjectMapSubgraph();

            base.RecursiveInitializeSubMapsFromCurrentGraph(refObjectMapNode);
        }

        #endregion

        #region Implementation of IRefObjectMapConfiguration

        public void AddJoinCondition(string childColumn, string parentColumn)
        {
            IBlankNode joinConditionNode = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(_refObjectMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrJoinCondition), joinConditionNode);
            R2RMLMappings.Assert(joinConditionNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrChild), R2RMLMappings.CreateLiteralNode(childColumn));
            R2RMLMappings.Assert(joinConditionNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrParent), R2RMLMappings.CreateLiteralNode(parentColumn));
        } 

        #endregion

        #region Implementation of IRefObjectMap

        public IEnumerable<JoinCondition> JoinConditions
        {
            get 
            {
                SparqlParameterizedString sparql = new SparqlParameterizedString(
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>
SELECT ?child ?parent
WHERE {
    @refObjectMap rr:joinCondition ?join .
    ?join rr:child ?child; rr:parent ?parent .
}");
                sparql.SetParameter("refObjectMap", _refObjectMapNode);
                SparqlResultSet result = (SparqlResultSet)R2RMLMappings.ExecuteQuery(sparql);

                foreach (var bindings in result)
                {
                    yield return new JoinCondition(bindings["child"].ToString(), bindings["parent"].ToString());
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

        public string EffectiveSqlQuery
        {
            get
            {
                return TriplesMap.R2RMLConfiguration.EffectiveSqlBuilder.GetEffectiveQueryForRefObjectMap(this);
            }
        }

        #endregion

        #region Implementation of IMapBase

        public override INode Node
        {
            get { return _refObjectMapNode; }
        }

        #endregion

        private void AssertObjectMapSubgraph()
        {
            R2RMLMappings.Assert(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), _refObjectMapNode);
            R2RMLMappings.Assert(_refObjectMapNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);
        }
    }
}
