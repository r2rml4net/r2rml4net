using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Interface for classes implementating the algorithm of mapping foreign keys to RDF predicates and obejcts
    /// </summary>
    public interface IForeignKeyMappingStrategy
    {
        /// <summary>
        /// Create a predicate URI for a foreign key
        /// </summary>
        Uri CreateReferencePredicateUri(Uri baseUri, ForeignKeyMetadata foreignKey);
        /// <summary>
        /// Create a URI template for referenced object
        /// </summary>
        string CreateReferenceObjectTemplate(Uri baseUri, ForeignKeyMetadata foreignKey);
        /// <summary>
        /// Create a blank node identifier template for referenced object
        /// </summary>
        string CreateObjectTemplateForCandidateKeyReference(ForeignKeyMetadata foreignKey);
    }
}