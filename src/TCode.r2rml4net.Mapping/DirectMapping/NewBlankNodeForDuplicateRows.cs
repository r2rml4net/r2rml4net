using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Produces invalid R2RML. Consider removind")]
    public class NewBlankNodeForDuplicateRows : DirectMappingStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        public NewBlankNodeForDuplicateRows()
            : this(new MappingOptions())
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public NewBlankNodeForDuplicateRows(MappingOptions options)
            : base(options)
        {
        }

        #region Overrides of DirectMappingStrategy
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectMap"></param>
        /// <param name="baseUri"></param>
        /// <param name="table"></param>
        public override void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode();
        }

        #endregion
    }
}