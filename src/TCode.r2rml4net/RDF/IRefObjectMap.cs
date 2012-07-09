using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.RDF
{
    public interface IRefObjectMap
    {
        IEnumerable<JoinCondition> JoinConditions { get; }
    }
}
