using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.RDB
{
    public interface IEffectiveSqlBuilder
    {
        string GetEffectiveQueryForTriplesMap(ITriplesMap triplesMap);
        string GetEffectiveQueryForRefObjectMap(IRefObjectMap refObjectMap);
    }
}