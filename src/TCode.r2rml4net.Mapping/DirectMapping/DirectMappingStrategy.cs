using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public class DirectMappingStrategy : MappingStrategyBase, IDirectMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;
        private IForeignKeyMappingStrategy _foreignKeyMappingStrategy;

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
            if (subjectMap == null)
                throw new ArgumentNullException("subjectMap");
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (table == null)
                throw new ArgumentNullException("table");

            if (table.PrimaryKey.Length != 0)
                throw new ArgumentException(string.Format("Table {0} has primay key. CreateSubjectMapForPrimaryKey method should be used", table.Name));

            string template = PrimaryKeyMappingStrategy.CreateSubjectTemplateForNoPrimaryKey(table);
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        public virtual void CreateSubjectMapForPrimaryKey(ISubjectMapConfiguration subjectMap, Uri baseUri, TableMetadata table)
        {
            if (subjectMap == null)
                throw new ArgumentNullException("subjectMap");
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (table == null)
                throw new ArgumentNullException("table");

            if(table.PrimaryKey.Length == 0)
                throw new ArgumentException(string.Format("Table {0} has no primay key", table.Name));

            var classIri = PrimaryKeyMappingStrategy.CreateSubjectUri(baseUri, table.Name);

            string template = PrimaryKeyMappingStrategy.CreateSubjectTemplateForPrimaryKey(baseUri, table);

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        public virtual void CreatePredicateMapForForeignKey(ITermMapConfiguration predicateMap, Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            Uri foreignKeyRefUri = ForeignKeyMappingStrategy.CreateReferencePredicateUri(baseUri, foreignKey);
            predicateMap.IsConstantValued(foreignKeyRefUri);
        }

        public virtual void CreateObjectMapForCandidateKeyReference(IObjectMapConfiguration objectMap, ForeignKeyMetadata foreignKey)
        {
            objectMap
                .IsTemplateValued(ForeignKeyMappingStrategy.CreateObjectTemplateForCandidateKeyReference(foreignKey))
                .IsBlankNode();
        }

        public virtual void CreateObjectMapForPrimaryKeyReference(IObjectMapConfiguration objectMap, Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            var templateForForeignKey = ForeignKeyMappingStrategy.CreateReferenceObjectTemplate(baseUri, foreignKey);
            objectMap.IsTemplateValued(templateForForeignKey);
        }

        #endregion

        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy(Options);

                return _primaryKeyMappingStrategy;
            }
            set { _primaryKeyMappingStrategy = value; }
        }

        public IForeignKeyMappingStrategy ForeignKeyMappingStrategy
        {
            get
            {
                if (_foreignKeyMappingStrategy == null)
                    _foreignKeyMappingStrategy = new ForeignKeyMappingStrategy(Options);

                return _foreignKeyMappingStrategy;
            }
            set { _foreignKeyMappingStrategy = value; }
        }
    }
}