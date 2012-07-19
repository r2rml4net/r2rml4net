namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Configuration for ref object maps described on http://www.w3.org/TR/r2rml/#dfn-referencing-object-map
    /// </summary>
    public interface IRefObjectMapConfiguration : IRefObjectMap
    {
        /// <summary>
        /// Adds a join condition, which will be used to join logical tables
        /// </summary>
        /// <param name="childColumn">column name that exists in the logical table of the triples map that contains the referencing object map</param>
        /// <param name="parentColumn">column name that exists in the logical table of the referencing object map's parent triples map</param>
        void AddJoinCondition(string childColumn, string parentColumn);
    }
}
