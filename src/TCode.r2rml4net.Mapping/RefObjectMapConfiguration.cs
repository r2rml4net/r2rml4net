using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping
{
    internal class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration
    {
        readonly ITriplesMap _parentTriplesMap;
        private readonly IPredicateObjectMap _predicateObjectMap;
        readonly ITriplesMapConfiguration _childTriplesMap;

        internal RefObjectMapConfiguration(IPredicateObjectMap predicateObjectMap, ITriplesMapConfiguration childTriplesMap, ITriplesMap parentTriplesMap, IGraph mappings)
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

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            AssertObjectMapSubgraph();
        }

        //protected internal override void RecursiveInitializeSubMapsFromCurrentGraph()
        //{
        //    R2RMLMappings.Retract(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), Node);
        //    R2RMLMappings.Retract(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);

        //    Node = refObjectMapNode;
        //    AssertObjectMapSubgraph();

        //    base.RecursiveInitializeSubMapsFromCurrentGraph();
        //}

        #endregion

        #region Implementation of IRefObjectMapConfiguration

        public void AddJoinCondition(string childColumn, string parentColumn)
        {
            IBlankNode joinConditionNode = R2RMLMappings.CreateBlankNode();

            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrJoinCondition), joinConditionNode);
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
                sparql.SetParameter("refObjectMap", Node);
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

        #endregion

        private void AssertObjectMapSubgraph()
        {
            R2RMLMappings.Assert(_predicateObjectMap.Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), Node);
            R2RMLMappings.Assert(Node, R2RMLMappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), _childTriplesMap.Node);
        }
    }
}
