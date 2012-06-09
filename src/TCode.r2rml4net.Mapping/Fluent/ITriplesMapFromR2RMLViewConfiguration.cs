using System;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Configuration of Triples Map, which source is a R2RML View as described on http://www.w3.org/TR/r2rml/#r2rml-views
    /// </summary>
    public interface ITriplesMapFromR2RMLViewConfiguration : ITriplesMapConfiguration
    {
        /// <summary>
        /// Sets the sql query to be conformat with a specific SQL language specification
        /// </summary>
        /// <param name="uri">Usually on of the URIs listed on http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs </param>
        ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(Uri uri);

        /// <summary>
        /// Sets the sql query to be conformat with a specific SQL language specification
        /// </summary>
        /// <param name="uri">String representation of the sql version URI. Usually on of the URIs listed on http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs </param>
        ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(string uri);

        /// <summary>
        /// Gets the URIs of SQL versions set for the logical table
        /// </summary>
        Uri[] SqlVersions { get; }
    }
}