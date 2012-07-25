using System.Data;
using TCode.r2rml4net.TriplesGeneration;

namespace TCode.r2rml4net.RDF
{
    internal class XSDLexicalFormProvider : ILexicalFormProvider
    {
        #region Implementation of ILexicalFormProvider

        public string GetLexicalForm(int columnIndex, IDataRecord logicalRow)
        {
            if (logicalRow.IsDBNull(columnIndex))
                return null;

            return logicalRow.GetValue(columnIndex).ToString();
        }

        #endregion
    }
}