using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DefaultMappingStrategy : MappingStrategyBase, IDirectMappingStrategy
    {
        private ISubjectMappingStrategy _subjectMappingStrategy;

        public DefaultMappingStrategy()
            : this(new DirectMappingOptions())
        {
        }

        public DefaultMappingStrategy(DirectMappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IDirectMappingStrategy

        public virtual void CreateSubjectMapForNoPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            string template = SubjectMappingStrategy.CreateSubjectTemplateForNoPrimaryKey(
                    table.Name,
                    table.Select(col => col.Name));
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        public virtual void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            var classIri = SubjectMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            string template = SubjectMappingStrategy.CreateSubjectTemplateForPrimaryKey(
                baseUri,
                table.Name,
                table.PrimaryKey.Select(pk => pk.Name));

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        #endregion

        public ISubjectMappingStrategy SubjectMappingStrategy
        {
            get
            {
                if(_subjectMappingStrategy == null)
                    _subjectMappingStrategy = new DefaultSubjectMapping(Options);

                return _subjectMappingStrategy;
            }
            set { _subjectMappingStrategy = value; }
        }
    }
}