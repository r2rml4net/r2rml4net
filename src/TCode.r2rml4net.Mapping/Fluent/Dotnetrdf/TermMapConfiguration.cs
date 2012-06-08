using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    class TermMapConfiguration : BaseConfiguration, ITermMapConfiguration, ISubjectMapConfiguration
    {
        internal INode TermMapNode { get; set; }

        internal TermMapConfiguration(IGraph r2RMLMappings)
            : base(r2RMLMappings)
        {
            TermMapNode = R2RMLMappings.CreateBlankNode();
        }

        #region Implementation of ITermMapConfiguration
        #endregion

        #region Implementation of ISubjectMapConfiguration

        public ISubjectMapConfiguration AddClass(Uri classIri)
        {
            return this;
        }

        public Uri[] ClassIris
        {
            get
            {
                return new Uri[0];
            }
        }

        #endregion
    }
}
