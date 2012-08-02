using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class NewBlankNodeForDuplicateRows : DefaultMappingStrategy
    {
        public NewBlankNodeForDuplicateRows()
            : this(new DirectMappingOptions())
        {

        }

        public NewBlankNodeForDuplicateRows(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Overrides of DefaultMappingStrategy

        public override void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode();
        }

        #endregion
    }
}