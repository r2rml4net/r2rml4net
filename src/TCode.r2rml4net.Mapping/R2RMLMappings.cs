using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping
{
    public class R2RMLMappings
    {
        IGraph _mappingsGraph;

        public R2RMLMappings()
        {
        }

        public TriplesMapLogicalTableBuilder CreateTriplesMap()
        {
            throw new NotImplementedException();
        }
    }
}
