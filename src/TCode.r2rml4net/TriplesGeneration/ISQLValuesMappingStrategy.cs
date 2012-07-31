using System;
using System.Data;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface ISQLValuesMappingStrategy
    {
        string GetLexicalForm(int columnIndex, IDataRecord logicalRow, out Uri naturalRdfDatatype);
    }
}