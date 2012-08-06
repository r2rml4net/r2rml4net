using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    [TestFixture]
    public class DirectMappingStrategyTests
    {
        static readonly Uri BaseUri = new Uri("http://example.com/base");

        private DirectMappingStrategy _strategy;
        private Mock<IForeignKeyMappingStrategy> _fkStrategy;
        private Mock<IPrimaryKeyMappingStrategy> _pkStrategy;
        private Mock<ISubjectMapConfiguration> _subjectMap;
        private Mock<ITermTypeConfiguration> _termType;

        [SetUp]
        public void Setup()
        {
            _fkStrategy = new Mock<IForeignKeyMappingStrategy>(MockBehavior.Strict);
            _pkStrategy = new Mock<IPrimaryKeyMappingStrategy>(MockBehavior.Strict);
            _termType = new Mock<ITermTypeConfiguration>(MockBehavior.Strict);
            _subjectMap = new Mock<ISubjectMapConfiguration>(MockBehavior.Strict);
            _subjectMap.Setup(sm => sm.TermType).Returns(_termType.Object);

            _strategy = new DirectMappingStrategy
                {
                    ForeignKeyMappingStrategy = _fkStrategy.Object,
                    PrimaryKeyMappingStrategy = _pkStrategy.Object
                };
        }

        [Test]
        public void InitializesSubjectMapForCorrectPrimaryKeyTable()
        {
            // given
            TableMetadata table = RelationalTestMappings.D006_1table1primarykey1column["Student"];
            var classIri = new Uri("http://example.com/Uri");
            _pkStrategy.Setup(pk => pk.CreateSubjectUri(BaseUri, "Student")).Returns(classIri);
            const string template = "some template";
            _pkStrategy.Setup(pk => pk.CreateSubjectTemplateForPrimaryKey(BaseUri, table)).Returns(template);
            _subjectMap.Setup(sm => sm.AddClass(classIri)).Returns(_subjectMap.Object);
            _subjectMap.Setup(sm => sm.IsTemplateValued(template)).Returns(_termType.Object);

            // when
            _strategy.CreateSubjectMapForPrimaryKey(_subjectMap.Object, BaseUri, table);

            // then
            _subjectMap.Verify(sm => sm.AddClass(classIri), Times.Once());
            _subjectMap.Verify(sm => sm.IsTemplateValued(template), Times.Once());
        }

        [Test]
        public void CannotInitializeSubjectMapWhenTableHasNoPrimaryKey()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];

            // then
            Assert.Throws<ArgumentException>(
                () => _strategy.CreateSubjectMapForPrimaryKey(_subjectMap.Object, BaseUri, table));
        }

        [Test]
        public void CannotInitializePrimaryKeySubjectMapWhenAnyParametersIsNull()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];

            // then
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForPrimaryKey(null, BaseUri, table));
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForPrimaryKey(_subjectMap.Object, null, table));
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForPrimaryKey(_subjectMap.Object, BaseUri, null));
        }

        [Test]
        public void InitializesSubjectMapForCorrectTableWithoutPrimary()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];
            var classIri = new Uri("http://example.com/Uri");
            _pkStrategy.Setup(pk => pk.CreateSubjectUri(BaseUri, "Student")).Returns(classIri);
            const string template = "some template";
            _pkStrategy.Setup(pk => pk.CreateSubjectTemplateForNoPrimaryKey(table)).Returns(template);
            _subjectMap.Setup(sm => sm.AddClass(classIri)).Returns(_subjectMap.Object);
            _subjectMap.Setup(sm => sm.IsTemplateValued(template)).Returns(_termType.Object);
            _termType.Setup(tt => tt.IsBlankNode()).Returns(_subjectMap.Object);

            // when
            _strategy.CreateSubjectMapForNoPrimaryKey(_subjectMap.Object, BaseUri, table);

            // then
            _subjectMap.Verify(sm => sm.AddClass(classIri), Times.Once());
            _subjectMap.Verify(sm => sm.IsTemplateValued(template), Times.Once());
            _subjectMap.Verify(sm => sm.TermType, Times.Once());
            _termType.Verify(tt => tt.IsBlankNode(), Times.Once());
        }

        [Test]
        public void CannotInitializeBlankNodeSubjectMapWhenTableHasPrimaryKey()
        {
            // given
            TableMetadata table = RelationalTestMappings.D006_1table1primarykey1column["Student"];

            // then
            Assert.Throws<ArgumentException>(
                () => _strategy.CreateSubjectMapForNoPrimaryKey(_subjectMap.Object, BaseUri, table));
        }

        [Test]
        public void CannotInitializeBlankNodeSubjectMapWhenAnyParametersIsNull()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];

            // then
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForNoPrimaryKey(null, BaseUri, table));
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForNoPrimaryKey(_subjectMap.Object, null, table));
            Assert.Throws<ArgumentNullException>(
                () => _strategy.CreateSubjectMapForNoPrimaryKey(_subjectMap.Object, BaseUri, null));
        }
    }
}