using System;
using System.IO;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping
{
    public static class R2RMLLoader
    {
        public static IR2RML Load(string r2RMLGraph)
        {
            throw new NotImplementedException();
        }

        public static IR2RML Load(Stream r2RMLGraph)
        {
            using(var reader  =new StreamReader(r2RMLGraph))
            {
                return Load(reader.ReadToEnd());
            }
        }
    }
}
