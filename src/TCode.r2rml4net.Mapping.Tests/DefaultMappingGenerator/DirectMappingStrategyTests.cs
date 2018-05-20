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
using System.Linq;
using Moq;
using Xunit;
using TCode.r2rml4net.Mapping.Direct;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.DefaultMappingGenerator
{
    public class DirectMappingStrategyTests
    {
        static readonly Uri BaseUri = new Uri("http://example.com/base");

        private DirectMappingStrategy _strategy;
        private Mock<IForeignKeyMappingStrategy> _fkStrategy;
        private Mock<IPrimaryKeyMappingStrategy> _pkStrategy;
        private Mock<ISubjectMapConfiguration> _subjectMap;
        private Mock<ITermTypeConfiguration> _termType;

        public DirectMappingStrategyTests()
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

        [Fact]
        public void InitializesSubjectMapForCorrectPrimaryKeyTable()
        {
            // given
            TableMetadata table = RelationalTestMappings.D006_1table1primarykey1column["Student"];
            var classIri = new Uri("http://example.com/Uri");
            _pkStrategy.Setup(pk => pk.CreateSubjectClassUri(BaseUri, "Student")).Returns(classIri);
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

        [Fact]
        public void CannotInitializeSubjectMapWhenTableHasNoPrimaryKey()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];

            // then
            Assert.Throws<ArgumentException>(
                () => _strategy.CreateSubjectMapForPrimaryKey(_subjectMap.Object, BaseUri, table));
        }

        [Fact]
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

        [Fact]
        public void InitializesSubjectMapForCorrectTableWithoutPrimary()
        {
            // given
            TableMetadata table = RelationalTestMappings.D003_1table3columns["Student"];
            var classIri = new Uri("http://example.com/Uri");
            _pkStrategy.Setup(pk => pk.CreateSubjectClassUri(BaseUri, "Student")).Returns(classIri);
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

        [Fact]
        public void CannotInitializeBlankNodeSubjectMapWhenTableHasPrimaryKey()
        {
            // given
            TableMetadata table = RelationalTestMappings.D006_1table1primarykey1column["Student"];

            // then
            Assert.Throws<ArgumentException>(
                () => _strategy.CreateSubjectMapForNoPrimaryKey(_subjectMap.Object, BaseUri, table));
        }

        [Fact]
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