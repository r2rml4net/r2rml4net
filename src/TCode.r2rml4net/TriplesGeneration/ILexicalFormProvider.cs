using System.Data;

namespace TCode.r2rml4net.TriplesGeneration
{
    public interface ILexicalFormProvider
    {
        string GetLexicalForm(int columnIndex, IDataRecord logicalRow);
    }
}