using System.Collections.Generic;
namespace TCode.r2rml4net.RDF
{
    public interface IPredicateObjectMap
    {
        IEnumerable<IObjectMap> ObjectMaps { get; }
        IEnumerable<IRefObjectMap> RefObjectMaps { get; }
    }
}