using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public interface IForeignKeyMappingStrategy
    {
        Uri CreateReferencePredicateUri(Uri baseUri, ForeignKeyMetadata foreignKey);
        string CreateReferenceObjectTemplate(Uri baseUri, ForeignKeyMetadata foreignKey);
    }
}