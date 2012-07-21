namespace TCode.r2rml4net.TriplesGeneration
{
    public interface IPredicateObjectMapProcessor
    {
        void ProcessPredicateObjectMap(Mapping.IPredicateObjectMap predicateObjectMap, System.Data.IDataRecord logicalRow);
    }
}