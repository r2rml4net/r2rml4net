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
using System.Threading;
using Xunit;

namespace TCode.r2rml4net.Tests
{
    public class MappingOptionsTests
    {
        private MappingOptions _options;

        public MappingOptionsTests()
        {
            _options = new MappingOptions();
        }

        [Fact]
        public void HasDefaultTemplateSeparator()
        {
            Assert.Equal("_", _options.BlankNodeTemplateSeparator);
        }

        [Fact]
        public void HasDefaultIdentifierDelimiter()
        {
            Assert.Equal('\"', _options.SqlIdentifierLeftDelimiter);
            Assert.Equal('\"', _options.SqlIdentifierRightDelimiter);
        }

        [Fact]
        public void DelimitsIdentifiersByDefault()
        {
            Assert.True(_options.UseDelimitedIdentifiers);
        }

        [Theory]
        [InlineData("")]
        [InlineData(":")]
        [InlineData("^_^")]
        public void DefaultTemplateSeparatorCanBeChanged(string newSeparator)
        {
            // when
            _options.WithBlankNodeTemplateSeparator(newSeparator);

            // then
            Assert.Equal(newSeparator, _options.BlankNodeTemplateSeparator);
        }

        [Fact]
        public void DefaultTemplateSeparatorCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _options.WithBlankNodeTemplateSeparator(null)
            );
        }

        [Theory]
        [InlineData('\"', '\"')]
        [InlineData('[', ']')]
        [InlineData('`', '`')]
        public void DefaultIdentifierDelimiterCanBeChanged(char newLeftDelimiter, char newRightDelimiter)
        {
            // when
            _options.WithSqlIdentifierDelimiters(newLeftDelimiter, newRightDelimiter);

            // then
            Assert.Equal(newLeftDelimiter, _options.SqlIdentifierLeftDelimiter);
            Assert.Equal(newRightDelimiter, _options.SqlIdentifierRightDelimiter);
        }

        [Fact]
        public void DefaultIdentifierDelimiterCanBeChangedToSameValue()
        {
            // when
            _options.WithSqlIdentifierDelimiters('-');

            // then
            Assert.Equal('-', _options.SqlIdentifierLeftDelimiter);
            Assert.Equal('-', _options.SqlIdentifierRightDelimiter);
        }

        [Fact]
        public void CanTurnOffIdentifierDelimiting()
        {
            // when
            _options.UsingDelimitedIdentifiers(false);

            // then
            Assert.False(_options.UseDelimitedIdentifiers);
        }

        [Fact]
        public void ValidateSqlVersionByDefault()
        {
            Assert.True(_options.ValidateSqlVersion);
        }

        [Fact]
        public void CanDisableSqlVersionValidatioon()
        {
            // when
            _options = _options.WithSqlVersionValidation(false);

            // then
            Assert.False(_options.ValidateSqlVersion);
        }

        [Fact]
        public void ByDefaultContinueOnErrors()
        {
            Assert.True(_options.IgnoreMappingErrors);
        }

        [Fact]
        public void ByDefaultDontPreserveDuplicateRows()
        {
            Assert.False(_options.PreserveDuplicateRows);
        }

        [Fact]
        public void Accessing_scope_without_inizializing_should_return_instance()
        {
            // when
            var mappingOptions = MappingOptions.Current;
            var mappingOptions2 = MappingOptions.Current;

            // then
            Assert.NotNull(mappingOptions);
            Assert.Same(mappingOptions, mappingOptions2);
        }

        [Fact]
        public void Creating_scope_should_change_current_mapping_options()
        {
            // when
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("x")))
            {
                Assert.Equal("x", MappingOptions.Current.BlankNodeTemplateSeparator);
            }

            // then
            Assert.Equal("_", MappingOptions.Current.BlankNodeTemplateSeparator);
            Assert.Equal("_", MappingOptions.Default.BlankNodeTemplateSeparator);
        }

        [Fact]
        public void Should_allow_nesting_scopes()
        {
            // when
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("x")))
            {
                using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("y")))
                {
                    Assert.Equal("y", MappingOptions.Current.BlankNodeTemplateSeparator);
                }
                Assert.Equal("x", MappingOptions.Current.BlankNodeTemplateSeparator);
            }

            // then
            Assert.Equal("_", MappingOptions.Current.BlankNodeTemplateSeparator);
            Assert.Equal("_", MappingOptions.Default.BlankNodeTemplateSeparator);
        }

        [Fact]
        public void Changed_options_should_not_affect_other_threads()
        {
            // given
            string inThreadSeparator = null;

            // when
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("x")))
            {
                var thread = new Thread(() =>
                    {
                        inThreadSeparator = MappingOptions.Current.BlankNodeTemplateSeparator;
                    });
                thread.Start();
                thread.Join();
            }

            // then
            Assert.Equal("_", inThreadSeparator);
        }

        [Fact]
        public void When_added_to_scope_should_be_frozen()
        {
             using (new MappingScope(new MappingOptions()))
             {
                 Assert.True(MappingOptions.Current.IsFrozen);
             }
        }

        [Fact]
        public void Default_options_should_be_frozen()
        {
            Assert.True(MappingOptions.Default.IsFrozen);
        }

        [Fact]
        public void Frozen_options_cannot_be_changed()
        {
            // when
            _options.Freeze();

            // then
            Assert.Throws<InvalidOperationException>(() => _options.IgnoringDataErrors(true));
            Assert.Throws<InvalidOperationException>(() => _options.UsingDelimitedIdentifiers(true));
            Assert.Throws<InvalidOperationException>(() => _options.WithAutomaticBlankNodeSubjects(true));
            Assert.Throws<InvalidOperationException>(() => _options.WithBlankNodeTemplateSeparator("_"));
            Assert.Throws<InvalidOperationException>(() => _options.WithDuplicateRowsPreserved(true));
            Assert.Throws<InvalidOperationException>(() => _options.WithSqlIdentifierDelimiters('[', ']'));
            Assert.Throws<InvalidOperationException>(() => _options.WithSqlVersionValidation(false));
            Assert.Throws<InvalidOperationException>(() => _options.IgnoringMappingErrors(false));
        }
    }
}