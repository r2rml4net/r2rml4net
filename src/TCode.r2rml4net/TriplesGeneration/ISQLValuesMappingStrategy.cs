using System;
using System.Data;

namespace TCode.r2rml4net.TriplesGeneration
{
    /// <summary>
    /// Strategy for mapping SQL column values to RDF datatypes
    /// </summary>
    public interface ISQLValuesMappingStrategy
    {
        /// <summary>
        /// Gets the column value's lexical form and it's RDF datatype URI
        /// </summary>
        string GetLexicalForm(int columnIndex, IDataRecord logicalRow, out Uri naturalRdfDatatype);
    }
}