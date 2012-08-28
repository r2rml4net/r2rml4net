using System;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Default implementation of <see cref="IDirectMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class DirectMappingStrategy : MappingStrategyBase, IDirectMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;
        private IForeignKeyMappingStrategy _foreignKeyMappingStrategy;

        /// <summary>
        /// Creates a new instance of <see cref="DirectMappingStrategy"/> with default options
        /// </summary>
        public DirectMappingStrategy()
            : this(new MappingOptions())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectMappingStrategy"/> with custom options
        /// </summary>
        public DirectMappingStrategy(MappingOptions options)
            : base(options)
        {
        }

        #region Implementation of IDirectMappingStrategy

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> as a template valued blank node with template 
        /// returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectTemplateForNoPrimaryKey"/>
        /// and class returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectClassUri"/>
        /// </summary>
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
            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, table.Name);

            // empty primary key generates blank node subjects
            subjectMap.AddClass(classIri).TermType.IsBlankNode().IsTemplateValued(template);
        }

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#subject-map">subject map</a> as a termplate valued URI noed with template 
        /// returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectTemplateForPrimaryKey"/>
        /// and class returned by <see cref="IPrimaryKeyMappingStrategy.CreateSubjectClassUri"/>
        /// </summary>
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

            var classIri = PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, table.Name);

            string template = PrimaryKeyMappingStrategy.CreateSubjectTemplateForPrimaryKey(baseUri, table);

            subjectMap.AddClass(classIri).IsTemplateValued(template);
        }

        /// <summary>
        /// Sets up a <a href="http://www.w3.org/TR/r2rml/#dfn-predicate-map">predicate map</a> as constant valued with URI returned by
        /// <see cref="IForeignKeyMappingStrategy.CreateReferencePredicateUri"/>
        /// </summary>
        public virtual void CreatePredicateMapForForeignKey(ITermMapConfiguration predicateMap, Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            Uri foreignKeyRefUri = ForeignKeyMappingStrategy.CreateReferencePredicateUri(baseUri, foreignKey);
            predicateMap.IsConstantValued(foreignKeyRefUri);
        }

        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> as template valued blank node with template
        /// returned by <see cref="IForeignKeyMappingStrategy.CreateObjectTemplateForCandidateKeyReference"/>
        /// </summary>
        public virtual void CreateObjectMapForCandidateKeyReference(IObjectMapConfiguration objectMap, ForeignKeyMetadata foreignKey)
        {
            objectMap
                .IsTemplateValued(ForeignKeyMappingStrategy.CreateObjectTemplateForCandidateKeyReference(foreignKey))
                .IsBlankNode();
        }

        /// <summary>
        /// Sets up an <a href="http://www.w3.org/TR/r2rml/#dfn-object-map">object map</a> as template valued node with template returned by
        /// <see cref="IForeignKeyMappingStrategy.CreateReferenceObjectTemplate"/>
        /// </summary>
        public virtual void CreateObjectMapForPrimaryKeyReference(IObjectMapConfiguration objectMap, Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            var templateForForeignKey = ForeignKeyMappingStrategy.CreateReferenceObjectTemplate(baseUri, foreignKey);
            objectMap.IsTemplateValued(templateForForeignKey);
        }

        #endregion

        /// <summary>
        /// Strategy for mapping primary keys
        /// </summary>
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

        /// <summary>
        /// Strategy for mapping foreign keys
        /// </summary>
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