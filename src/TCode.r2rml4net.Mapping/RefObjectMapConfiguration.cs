using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using TCode.r2rml4net.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping
{
    public class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration, IRefObjectMap
    {
        INode _refObjectMapNode;

        public RefObjectMapConfiguration(INode predicateObjectMapNode, INode referencedTriplesMap, IGraph mappings)
            : base(mappings)
        {
            _refObjectMapNode = mappings.CreateBlankNode();
            mappings.Assert(predicateObjectMapNode, mappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), _refObjectMapNode);
            mappings.Assert(_refObjectMapNode, mappings.CreateUriNode(R2RMLUris.RrParentTriplesMapProperty), referencedTriplesMap);
        }

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            throw new NotImplementedException();
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

        #endregion
    }
}
