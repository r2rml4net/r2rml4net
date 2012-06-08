using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net
{
    /// <summary>
    /// Part of the Visitor pattern, here used to visit database metadata elements
    /// in order to create R2RML mappings
    /// </summary>
    /// <typeparam name="TVisitor">the Visitor type</typeparam>
    public interface IVistitable<TVisitor>
    {
        /// <summary>
        /// Invoke the Visit method and forward visiting to child elements (if any)
        /// </summary>
        void Accept(TVisitor visitor);
    }
}
