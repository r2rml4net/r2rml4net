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
            IBlankNode @object = t.Object as IBlankNode;
            Triple toHandle = t;

            IBlankNode replacedSubject = null, replacedObject = null;
            if (subject != null)
            {
                if (!_replacedNodes.ContainsKey(subject.InternalID))
                {
                    _replacedNodes.Add(subject.InternalID, _wrapped.CreateBlankNode());
                }
                replacedSubject = _replacedNodes[subject.InternalID];
            }
            if (@object !=null)
            {
                if (!_replacedNodes.ContainsKey(@object.InternalID))
                {
                    _replacedNodes.Add(@object.InternalID, _wrapped.CreateBlankNode());
                }
                replacedObject = _replacedNodes[@object.InternalID];
            }

            if (replacedSubject != null || replacedObject != null)
                toHandle = t.CloneTriple(replacedSubject: replacedSubject, replacedObject: replacedObject);

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

        protected override void EndRdfInternal(bool ok)
        {
            _wrapped.EndRdf(ok);
            base.EndRdfInternal(ok);
        }

        #endregion
    }
}