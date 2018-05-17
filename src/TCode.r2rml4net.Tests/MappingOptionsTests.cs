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
using NUnit.Framework;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class MappingOptionsTests
    {
        private MappingOptions _options;

        [SetUp]
        public void Setup()
        {
            _options = new MappingOptions();
        }

        [Test]
        public void HasDefaultTemplateSeparator()
        {
            Assert.AreEqual("_", _options.BlankNodeTemplateSeparator);
        }

        [Test]
        public void HasDefaultIdentifierDelimiter()
        {
            Assert.AreEqual('\"', _options.SqlIdentifierLeftDelimiter);
            Assert.AreEqual('\"', _options.SqlIdentifierRightDelimiter);
        }

        [Test]
        public void DelimitsIdentifiersByDefault()
        {
            Assert.AreEqual(true, _options.UseDelimitedIdentifiers);
        }

        [TestCase("")]
        [TestCase(":")]
        [TestCase("^_^")]
        public void DefaultTemplateSeparatorCanBeChanged(string newSeparator)
        {
            // when
            _options.WithBlankNodeTemplateSeparator(newSeparator);

            // then
            Assert.AreEqual(newSeparator, _options.BlankNodeTemplateSeparator);
        }

        [Test]
        public void DefaultTemplateSeparatorCannotBeNull(string newSeparator)
        {
            Assert.Throws<ArgumentNullException>(() =>
                _options.WithBlankNodeTemplateSeparator(null)
            );
        }

        [TestCase('\"', '\"')]
        [TestCase('[', ']')]
        [TestCase('`', '`')]
        public void DefaultIdentifierDelimiterCanBeChanged(char newLeftDelimiter, char newRightDelimiter)
        {
            // when
            _options.WithSqlIdentifierDelimiters(newLeftDelimiter, newRightDelimiter);

            // then
            Assert.AreEqual(newLeftDelimiter, _options.SqlIdentifierLeftDelimiter);
            Assert.AreEqual(newRightDelimiter, _options.SqlIdentifierRightDelimiter);
        }

        [Test]
        public void DefaultIdentifierDelimiterCanBeChangedToSameValue()
        {
            // when
            _options.WithSqlIdentifierDelimiters('-');

            // then
            Assert.AreEqual('-', _options.SqlIdentifierLeftDelimiter);
            Assert.AreEqual('-', _options.SqlIdentifierRightDelimiter);
        }

        [Test]
        public void CanTurnOffIdentifierDelimiting()
        {
            // when
            _options.UsingDelimitedIdentifiers(false);

            // then
            Assert.AreEqual(false, _options.UseDelimitedIdentifiers);
        }

        [Test]
        public void ValidateSqlVersionByDefault()
        {
            Assert.IsTrue(_options.ValidateSqlVersion);
        }

        [Test]
        public void CanDisableSqlVersionValidatioon()
        {
            // when
            _options = _options.WithSqlVersionValidation(false);

            // then
            Assert.IsFalse(_options.ValidateSqlVersion);
        }

        [Test]
        public void ByDefaultContinueOnErrors()
        {
            Assert.IsTrue(_options.IgnoreMappingErrors);
        }

        [Test]
        public void ByDefaultDontPreserveDuplicateRows()
        {
            Assert.IsFalse(_options.PreserveDuplicateRows);
        }

        [Test]
        public void Accessing_scope_without_inizializing_should_return_instance()
        {
            // when
            var mappingOptions = MappingOptions.Current;
            var mappingOptions2 = MappingOptions.Current;

            // then
            Assert.That(mappingOptions, Is.Not.Null);
            Assert.That(mappingOptions, Is.SameAs(mappingOptions2));
        }

        [Test]
        public void Creating_scope_should_change_current_mapping_options()
        {
            // when
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("x")))
            {
                Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("x"));
            }

            // then
            Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("_"));
            Assert.That(MappingOptions.Default.BlankNodeTemplateSeparator, Is.EqualTo("_"));
        }

        [Test]
        public void Should_allow_nesting_scopes()
        {
            // when
            using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("x")))
            {
                using (new MappingScope(new MappingOptions().WithBlankNodeTemplateSeparator("y")))
                {
                    Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("y"));
                }
                Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("x"));
            }

            // then
            Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("_"));
            Assert.That(MappingOptions.Default.BlankNodeTemplateSeparator, Is.EqualTo("_"));
        }

        [Test]
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
            Assert.That(inThreadSeparator, Is.EqualTo("_"));
        }

        [Test]
        public void When_added_to_scope_should_be_frozen()
        {
             using (new MappingScope(new MappingOptions()))
             {
                 Assert.That(MappingOptions.Current.IsFrozen, Is.True);
             }
        }

        [Test]
        public void Default_options_should_be_frozen()
        {
            Assert.That(MappingOptions.Default.IsFrozen, Is.True);
        }

        [Test]
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