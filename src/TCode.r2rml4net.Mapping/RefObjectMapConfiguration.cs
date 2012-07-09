using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping
{
    public class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration, IRefObjectMap
    {
        public RefObjectMapConfiguration(INode predicateObjectMapNode, IGraph mappings)
            : base(mappings)
        {
        }

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
