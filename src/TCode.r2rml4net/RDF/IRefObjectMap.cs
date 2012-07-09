using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Represents a RefObjectMap
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-referencing-object-map</remarks>
    public interface IRefObjectMap
    {
        /// <summary>
        /// Optional join conditions associated with this <see cref="IRefObjectMap"/>
        /// </summary>
        IEnumerable<JoinCondition> JoinConditions { get; }
    }
}
