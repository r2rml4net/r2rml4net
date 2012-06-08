using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent.Dotnetrdf;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests
{
    [TestFixture]
    public class TermMapConfigurationTests
    {
        TermMapConfiguration _termMapConfiguration;

        [SetUp]
        public void Setup()
        {
            _termMapConfiguration = new TermMapConfiguration(new R2RMLConfiguration().R2RMLMappings);
        }

        [Test]
        public void CanAddMultipleClassIrisToSubjectMap()
        {
            // given
            Uri class1 = new Uri("http://example.com/ontology#class");
            Uri class2 = new Uri("http://example.com/ontology#anotherClass");
            Uri class3 = new Uri("http://semantic.mobi/resource/yetAnother");

            // when
            _termMapConfiguration.AddClass(class1).AddClass(class2).AddClass(class3);

            // then
            Assert.AreEqual(3, _termMapConfiguration.ClassIris.Length);
            Assert.Contains(class1, _termMapConfiguration.ClassIris);
            Assert.Contains(class2, _termMapConfiguration.ClassIris);
            Assert.Contains(class3, _termMapConfiguration.ClassIris);
        }

        [Test]
        public void ConstructorCreatesNodeForTheTermMapIsRepresents()
        {
            // given
            IGraph mappings = new Graph();

            // when
            var configuration = new TermMapConfiguration(mappings);

            // then
            Assert.IsNotNull(configuration.TermMapNode);
            Assert.AreSame(mappings, configuration.TermMapNode.Graph);
        }
    }
}
