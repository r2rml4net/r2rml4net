using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    [Obsolete("Remove?")]
    public class NewBlankNodeForDuplicateRows : DirectMappingStrategy
    {
        public NewBlankNodeForDuplicateRows()
            : this(new DirectMappingOptions())
        {

        }

        public NewBlankNodeForDuplicateRows(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Overrides of DirectMappingStrategy

        public override void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode();
        }

        #endregion
    }
}