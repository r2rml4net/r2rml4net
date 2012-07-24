using System.Data;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface INaturalLexicalFormProvider
    {
        string GetNaturalLexicalForm(int columnIndex, IDataRecord logicalRow);
    }
}