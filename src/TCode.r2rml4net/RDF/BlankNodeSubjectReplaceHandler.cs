using System.Collections.Generic;
using VDS.Common;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;

namespace TCode.r2rml4net.RDF
{
    internal class BlankNodeSubjectReplaceHandler : BaseRdfHandler
    {
        private readonly IRdfHandler _wrapped;
        private readonly IDictionary<string, IBlankNode> _replacedNodes = new HashTable<string, IBlankNode>();

        public BlankNodeSubjectReplaceHandler(IRdfHandler wrapped)
        {
            _wrapped = wrapped;
        }

        #region Overrides of BaseRdfHandler

        /// <summary>
        /// Must be overridden by derived handlers to take appropriate Triple handling action
        /// </summary>
        /// <param name="t">Triple</param>
        /// <returns/>
        protected override bool HandleTripleInternal(Triple t)
        {
            IBlankNode subject = t.Subject as IBlankNode;
            Triple toHandle = t;

            if (subject != null)
            {
                if (!_replacedNodes.ContainsKey(subject.InternalID))
                {
                    _replacedNodes.Add(subject.InternalID, _wrapped.CreateBlankNode());
                }
                IBlankNode replacedSubject = _replacedNodes[subject.InternalID];

                toHandle = t.CloneTriple(replacedSubject);
            }

            return _wrapped.HandleTriple(toHandle);
        }

        /// <summary>
        /// Gets whether the Handler will accept all Triples i.e. it will never abort handling early
        /// </summary>
        public override bool AcceptsAll
        {
            get { return true; }
        }

        protected override void StartRdfInternal()
        {
            _wrapped.StartRdf();
            base.StartRdfInternal();
        }

        #endregion
    }
}