using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class TermMapConfigurationTests
    {
        private INode _triplesMapNode;
        private Mock<TermMapConfiguration> _termMapConfigurationMock;
        private TermMapConfiguration _termMapConfiguration;
        private IGraph _graph;
        private Mock<ITriplesMapConfiguration> _parentTriplesMap;

        [SetUp]
        public void Setup()
        {
            _graph = new R2RMLConfiguration().R2RMLMappings;
            _triplesMapNode = _graph.CreateUriNode(new Uri("http://mapping.com/SomeMap"));
            _parentTriplesMap = new Mock<ITriplesMapConfiguration>();
            _parentTriplesMap.Setup(tm => tm.Node).Returns(_triplesMapNode);
            Mock<IMapBase> parentMap = new Mock<IMapBase>();
            parentMap.Setup(map => map.Node).Returns(_graph.CreateBlankNode());

            _termMapConfigurationMock = new Mock<TermMapConfiguration>(_parentTriplesMap.Object, parentMap.Object, _graph)
                                            {
                                                CallBase = true
                                            };
            _termMapConfigurationMock
                .Setup(config => config.CreateMapPropertyNode())
                .Returns(_graph.CreateUriNode(new Uri(UriConstants.RrSubjectMapProperty)));
            _termMapConfiguration = _termMapConfigurationMock.Object;
        }

        [Test]
        public void ConstantIRIValueCanBeSetOnlyOnce()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _termMapConfiguration.IsConstantValued(uri);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.IsConstantValued(uri));
        }

        public void CanBeConstantIRIValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _termMapConfiguration.IsConstantValued(uri);

            // then
            Assert.AreEqual(uri, _termMapConfiguration.ConstantValue);
        }

        [Test]
        public void TermMapCanBeColumnValued()
        {
            // given
            const string columnName = "Name";

            // when
            _termMapConfiguration.IsColumnValued(columnName);

            // then
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode,
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrColumnProperty)),
                _termMapConfiguration.R2RMLMappings.CreateLiteralNode(columnName))));
            Assert.AreEqual(UriConstants.RrIRI, _termMapConfiguration.TermTypeURI.ToString());
            Assert.AreEqual(columnName, _termMapConfiguration.ColumnName);
        }

        [Test]
        public void ColumnValueCanOnlyBeSetOnce()
        {
            // given
            const string columnName = "Name";

            // when
            _termMapConfiguration.IsColumnValued(columnName);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.IsColumnValued(columnName));
        }

        [Test]
        public void TermMapCanBeTemplateValued()
        {
            // given
            const string template = @"\\{\\{\\{ \\\\o/ {TITLE} \\\\o/ \\}\\}\\}";

            // when
            _termMapConfiguration.IsTemplateValued(template);

            //then
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode,
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrTemplateProperty)),
                _termMapConfiguration.R2RMLMappings.CreateLiteralNode(template))));
            Assert.AreEqual(UriConstants.RrIRI, _termMapConfiguration.TermTypeURI.ToString());
            Assert.AreEqual(template, _termMapConfiguration.Template);
        }

        [Test]
        public void TemplateCanOnlyBeSetOnce()
        {
            // given
            const string template = @"\\{\\{\\{ \\\\o/ {TITLE} \\\\o/ \\}\\}\\}";

            // when
            _termMapConfiguration.IsTemplateValued(template);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.IsTemplateValued(template));
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.IsTemplateValued("something else"));
        }

        [Test]
        public void CanBeOfTypeBlankNode()
        {
            // when
            _termMapConfiguration.TermType.IsBlankNode();

            // then
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.ContainsTriple(new Triple(
                _termMapConfiguration.ParentMapNode, 
                _termMapConfiguration.CreateMapPropertyNode(),
                _termMapConfiguration.Node)));
            Assert.IsTrue(_termMapConfiguration.R2RMLMappings.GetTriplesWithSubjectPredicate(
                _termMapConfiguration.Node,
                _termMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrTermTypeProperty))).Any());
            Assert.AreEqual(UriConstants.RrBlankNode, _termMapConfiguration.TermTypeURI.ToString());
        }

        [Test]
        public void ColumnNameIsNullByDefault()
        {
            Assert.IsNull(_termMapConfiguration.ColumnName);
        }

        [Test]
        public void TemplateIsNullByDefault()
        {
            Assert.IsNull(_termMapConfiguration.Template);
        }

        [Test]
        public void ConstantValueIsNullByDefault()
        {
            Assert.IsNull(_termMapConfiguration.ConstantValue);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MustBeInitializedWithANode()
        {
            _termMapConfiguration.RecursiveInitializeSubMapsFromCurrentGraph(null);
        }

        [Test]
        public void CanHaveInverseExpression()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.SetInverseExpression(expression);

            // then
            Assert.AreEqual(expression, _termMapConfiguration.InverseExpression);
        }

        [Test]
        public void CannotHaveInverseExpressionWhenConstantValued()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.IsConstantValued(new Uri("http://www.example.com/TermUri"));

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.SetInverseExpression(expression));
        }

        [Test]
        public void CannotConstantValueWhenInverseExpressionIsSet()
        {
            // given
            const string expression = "{DEPTNO} = SUBSTRING({DEPTID}, CHARACTER_LENGTH('Department')+1)";

            // when
            _termMapConfiguration.SetInverseExpression(expression);

            // then
            Assert.Throws<InvalidTriplesMapException>(() => _termMapConfiguration.IsConstantValued(new Uri("http://www.example.com/TermUri")));
        }
    }
}
