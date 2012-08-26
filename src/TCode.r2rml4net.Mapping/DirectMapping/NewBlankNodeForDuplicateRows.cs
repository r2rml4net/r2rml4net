using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    [Obsolete("Produces invalid R2RML. Consider removind")]
    public class NewBlankNodeForDuplicateRows : DirectMappingStrategy
    {
        public NewBlankNodeForDuplicateRows()
            : this(new MappingOptions())
        {

        }

        public NewBlankNodeForDuplicateRows(MappingOptions options)
            : base(options)
        {
        }

        #region Overrides of DirectMappingStrategy

        public override void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode();
        }

        #endregion
    }
}