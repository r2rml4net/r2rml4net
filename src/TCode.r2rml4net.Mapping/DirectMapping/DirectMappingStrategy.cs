using System;
using System.Linq;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DirectMappingStrategy : MappingStrategyBase, IDirectMappingStrategy
    {
        private ISubjectMappingStrategy _subjectMappingStrategy;

        public DirectMappingStrategy()
            : this(new DirectMappingOptions())
        {
        }

        public DirectMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IDirectMappingStrategy

        public virtual void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            string template = SubjectMappingStrategy.CreateSubjectTemplateForNoPrimaryKey(table);
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        public virtual void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table);

            string template = SubjectMappingStrategy.CreateSubjectTemplateForPrimaryKey(baseUri, table);

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        #endregion

        public ISubjectMappingStrategy SubjectMappingStrategy
        {
            get
            {
                if(_subjectMappingStrategy == null)
                    _subjectMappingStrategy = new SubjectMappingStrategy(Options);

                return _subjectMappingStrategy;
            }
            set { _subjectMappingStrategy = value; }
        }
    }
}