using System.Data;

namespace TCode.r2rml4net.TriplesGeneration
{
    internal class W3CLexicalFormProvider : INaturalLexicalFormProvider
    {
        #region Implementation of INaturalLexicalFormProvider

        public string GetNaturalLexicalForm(int columnIndex, IDataRecord logicalRow)
        {
            return "Test";
        }

        #endregion
    }
}