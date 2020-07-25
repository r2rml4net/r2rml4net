#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
//
// ------------------------------------------------------------------------
//
// This file is part of r2rml4net.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.
//
// ------------------------------------------------------------------------
//
// r2rml4net may alternatively be used under the LGPL licence
//
// http://www.gnu.org/licenses/lgpl.html
//
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion
using System;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    public class SubjectMapConfigurationTests
    {
        private readonly IGraph _graph;
        private SubjectMapConfiguration _subjectMapConfiguration;
        private readonly IUriNode _triplesMapNode;
        private readonly Mock<ITriplesMapConfiguration> _triplesMap;

        public SubjectMapConfigurationTests()
        {
            _graph = new FluentR2RML(new MappingOptions()).R2RMLMappings;
            _triplesMapNode = _graph.CreateUriNode(new Uri("http://unittest.mappings.com/TriplesMap"));

            _triplesMap = new Mock<ITriplesMapConfiguration>();
            _triplesMap.Setup(tm => tm.Node).Returns(_triplesMapNode);
            _subjectMapConfiguration = new SubjectMapConfiguration(_triplesMap.Object, _graph);
        }

        [Fact]
        public void CanAddMultipleClassIrisToSubjectMap()
        {
            // given
            Uri class1 = new Uri("http://example.com/ontology#class");
            Uri class2 = new Uri("http://example.com/ontology#anotherClass");
            Uri class3 = new Uri("http://semantic.mobi/resource/yetAnother");

            // when
            _subjectMapConfiguration.AddClass(class1).AddClass(class2).AddClass(class3);

            // then
            Assert.Equal(3, _subjectMapConfiguration.Classes.Length);
            Assert.Contains(class1, _subjectMapConfiguration.Classes);
            Assert.Contains(class2, _subjectMapConfiguration.Classes);
            Assert.Contains(class3, _subjectMapConfiguration.Classes);
        }

        [Fact]
        public void CanSetTermMapsTermTypeToIRI()
        {
            // when
            _subjectMapConfiguration.TermType.IsIRI();

            // then
            Assert.Equal(UriConstants.RrIRI, _subjectMapConfiguration.TermTypeURI.AbsoluteUri);
            _subjectMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrTermTypeProperty, UriConstants.RrIRI);
        }

        [Fact]
        public void CanSetTermMapsTermTypeToBlankNode()
        {
            // when
            _subjectMapConfiguration.TermType.IsBlankNode();

            // then
            Assert.Equal(UriConstants.RrBlankNode, _subjectMapConfiguration.TermTypeURI.AbsoluteUri);
            _subjectMapConfiguration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrTermTypeProperty, UriConstants.RrBlankNode);
        }

        [Fact]
        public void CannnotSetTermMapsTermTypeToLiteral()
        {
            // when
            Assert.Throws<InvalidMapException>(() => _subjectMapConfiguration.TermType.IsLiteral());
        }

        [Fact]
        public void DefaultTermTypeIsIRI()
        {
            Assert.Equal(UriConstants.RrIRI, _subjectMapConfiguration.TermTypeURI.AbsoluteUri);
        }

        [Fact]
        public void CannoSetTermTypeTwice()
        {
            // when
            _subjectMapConfiguration.TermType.IsIRI();

            // then
            Assert.Throws<InvalidMapException>(() => _subjectMapConfiguration.TermType.IsIRI());
            Assert.Throws<InvalidMapException>(() => _subjectMapConfiguration.TermType.IsBlankNode());
        }

        [Fact]
        public void SubjectMapCanBeIRIConstantValued()
        {
            // given
            Uri uri = new Uri("http://example.com/SomeResource");

            // when
            _subjectMapConfiguration.IsConstantValued(uri);

            // then
            Assert.True(_subjectMapConfiguration.R2RMLMappings.ContainsTriple(
                new Triple(
                    _subjectMapConfiguration.ParentMapNode,
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrSubjectMapProperty)),
                    _subjectMapConfiguration.Node)));
            Assert.True(_subjectMapConfiguration.R2RMLMappings.ContainsTriple(
                new Triple(
                    _subjectMapConfiguration.Node,
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(new Uri(UriConstants.RrConstantProperty)),
                    _subjectMapConfiguration.R2RMLMappings.CreateUriNode(uri))));
            Assert.Equal(uri, _subjectMapConfiguration.URI);
        }

        [Fact]
        public void CanHaveClassesAndBeTemplateValued()
        {
            // given
            Uri class1 = new Uri("http://example.com/ontology#class");
            const string template = "http://www.example.com/res/{column}";

            // when
            _subjectMapConfiguration.AddClass(class1).IsTemplateValued(template);

            // then
            Assert.Contains(class1, _subjectMapConfiguration.Classes);
            Assert.Equal(template, _subjectMapConfiguration.Template);
        }

        [Fact]
        public void SubjectIsNullByDefault()
        {
            Assert.Null(_subjectMapConfiguration.URI);
        }

        [Fact]
        public void CanCreateMultipleGraphMap()
        {
            // when
            IGraphMap graphMap1 = _subjectMapConfiguration.CreateGraphMap();
            IGraphMap graphMap2 = _subjectMapConfiguration.CreateGraphMap();

            // then
            Assert.NotSame(graphMap1, graphMap2);
            Assert.True(graphMap1 is TermMapConfiguration);
            Assert.True(graphMap2 is TermMapConfiguration);
        }

        [Fact]
        public void CreatesCorrectShortcutPropertyNode()
        {
            Assert.Equal(new Uri("http://www.w3.org/ns/r2rml#subject"), _subjectMapConfiguration.CreateShortcutPropertyNode().Uri);
        }

        [Fact]
        public void NodeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _subjectMapConfiguration = new SubjectMapConfiguration(_triplesMap.Object, _graph, (INode)null)
            );
        }
    }
}
