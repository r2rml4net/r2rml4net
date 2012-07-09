﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping
{
    public class RefObjectMapConfiguration : BaseConfiguration, IRefObjectMapConfiguration, IRefObjectMap
    {
        INode _refObjectMapNode;

        public RefObjectMapConfiguration(INode predicateObjectMapNode, IGraph mappings)
            : base(mappings)
        {
            _refObjectMapNode = mappings.CreateBlankNode();
            mappings.Assert(predicateObjectMapNode, mappings.CreateUriNode(R2RMLUris.RrObjectMapProperty), _refObjectMapNode);
        }

        #region Overrides of BaseConfiguration

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
