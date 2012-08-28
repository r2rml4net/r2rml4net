using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Interface for classes implementating the algorithm of direct mapping relational databases
    /// to <a href="http://www.w3.org/TR/r2rml/#subject-map">subject maps</a>, 
    /// <a href="http://www.w3.org/TR/r2rml/#dfn-predicate-map">predicate maps</a>
    /// and <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object maps</a>
    /// </summary>
    public interface IDirectMappingStrategy
    {
        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> for table without primary key
        /// </summary>
        void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table);
        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> for table with primary key
        /// </summary>
        void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table);
        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#dfn-predicate-map">predicate map</a> for foreign key
        /// </summary>
        void CreatePredicateMapForForeignKey(ITermMapConfiguration predicateMap, Uri baseUri, ForeignKeyMetadata foreignKey);
        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> for candidate key reference
        /// </summary>
        void CreateObjectMapForCandidateKeyReference(IObjectMapConfiguration createObjectMap, ForeignKeyMetadata foreignKey);
        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> for primary key reference
        /// </summary>
        void CreateObjectMapForPrimaryKeyReference(IObjectMapConfiguration objectMap, Uri baseUri, ForeignKeyMetadata foreignKey);
    }
}