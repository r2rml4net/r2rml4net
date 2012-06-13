using System;
using System.Globalization;
using System.Linq;
using TCode.r2rml4net.RDF;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Fluent.Dotnetrdf
{
    internal class ObjectMapConfiguration : TermMapConfiguration, IObjectMapConfiguration, ILiteralTermMapConfiguration
    {
        internal ObjectMapConfiguration(INode triplesMapNode, IGraph r2RMLMappings) : base(triplesMapNode, r2RMLMappings)
        {
        }

        #region Implementation of IObjectMapConfiguration

        public ILiteralTermMapConfiguration IsConstantValued(string literal)
        {
            CheckRelationWithParentMap(true);

            if (R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, CreateConstantPropertyNode()).Any())
                throw new InvalidTriplesMapException("Term map can have at most one constant value");

            R2RMLMappings.Assert(ParentMapNode, CreateConstantPropertyNode(), R2RMLMappings.CreateLiteralNode(literal));

            return this;
        }

        ILiteralTermMapConfiguration IObjectMapConfiguration.IsColumnValued(string columnName)
        {
            IsColumnValued(columnName);
            return this;
        }

        #endregion

        #region Overrides of TermMapConfiguration

        protected internal override IUriNode CreateConstantPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(UrisHelper.RrObjectProperty);
        }

        protected internal override IUriNode CreateMapPropertyNode()
        {
            return R2RMLMappings.CreateUriNode(UrisHelper.RrObjectMapProperty);
        }

        /// <summary>
        /// Overriden, because object maps can be of term type rr:Literal
        /// </summary>
        public override Uri URI
        {
            get
            {
                if (ExplicitTermType != null)
                    return ExplicitTermType;

                // term type is literal is column valued, or has datatype or language tag
                if (IsLiteralTermType)
                    return R2RMLMappings.CreateUriNode(UrisHelper.RrLiteral).Uri;

                // in other cases is rr:IRI
                return R2RMLMappings.CreateUriNode(UrisHelper.RrIRI).Uri;
            }
        }

        /// <summary>
        /// See http://www.w3.org/TR/r2rml/#termtype
        /// </summary>
        private bool IsLiteralTermType
        {
            get
            {
                var columnPropertyNode = R2RMLMappings.CreateUriNode(UrisHelper.RrColumnProperty);
                var columnTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, columnPropertyNode).ToArray();

                if (columnTriples.Any())
                    return true;

                var languageTagNode = R2RMLMappings.CreateUriNode(UrisHelper.RrLanguageTagPropety);
                var languageTagTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, languageTagNode).ToArray();

                var datatypeNode = R2RMLMappings.CreateUriNode(UrisHelper.RrDatatypePropety);
                var datatypeTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, datatypeNode).ToArray();

                if (languageTagTriples.Any() && datatypeTriples.Any())
                    throw new InvalidTriplesMapException("Object map cannot have both a rr:languageTag and rr:datatype properties set");

                return datatypeTriples.Any() || languageTagTriples.Any();
            }
        }

        public override ITermMapConfiguration IsLiteral()
        {
            AssertTermTypeNotSet();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrTermTypeProperty), R2RMLMappings.CreateUriNode(UrisHelper.RrLiteral));
            return this;
        }

        #endregion

        #region Implementation of ILiteralTermMapConfiguration

        public void HasDataType(string dataTypeUri)
        {
            HasDataType(new Uri(dataTypeUri));
        }

        public void HasDataType(Uri dataTypeUri)
        {
            EnsureOnlyLanguageTagOrDatatype();
            ReplaceShortcutWithWithMapProperty();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrDatatypePropety), R2RMLMappings.CreateUriNode(dataTypeUri));
        }

        public void HasLanguageTag(string languagTag)
        {
            EnsureOnlyLanguageTagOrDatatype();
            ReplaceShortcutWithWithMapProperty();

            R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrLanguageTagPropety), R2RMLMappings.CreateLiteralNode(languagTag.ToLower()));
        }

        public void HasLanguageTag(CultureInfo cultureInfo)
        {
            HasLanguageTag(cultureInfo.Name);
        }

        private void ReplaceShortcutWithWithMapProperty()
        {
            var shortcutTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(ParentMapNode, CreateConstantPropertyNode()).ToArray();

            if(shortcutTriples.Length > 1)
                throw new InvalidTriplesMapException("Predicated object map contains multiple constant object maps");

            if(shortcutTriples.Any())
            {
                Triple shortcutTriple = shortcutTriples[0];
                R2RMLMappings.Retract(shortcutTriple);
                R2RMLMappings.Assert(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrConstantProperty), shortcutTriple.Object);
                CheckRelationWithParentMap();
            }
        }

        private void EnsureOnlyLanguageTagOrDatatype()
        {
            var datatypeTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrDatatypePropety));
            var languageTagTriples = R2RMLMappings.GetTriplesWithSubjectPredicate(TermMapNode, R2RMLMappings.CreateUriNode(UrisHelper.RrLanguageTagPropety));

            if (datatypeTriples.Any())
                throw new InvalidTriplesMapException("Object map already has a datatype");
            if (languageTagTriples.Any())
                throw new InvalidTriplesMapException("Object map already has a language tag");
        }

        #endregion
    }
}