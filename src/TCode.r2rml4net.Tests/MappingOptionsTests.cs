#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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

        [TestCase(null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("")]
        [TestCase(":")]
        [TestCase("^_^")]
        public void DefaultTemplateSeparatorCanBeChanged(string newSeparator)
        {
            // when
            _options.BlankNodeTemplateSeparator = newSeparator;

            // then
            Assert.AreEqual(newSeparator, _options.BlankNodeTemplateSeparator);
        }

        [TestCase('\"', '\"')]
        [TestCase('[', ']')]
        [TestCase('`', '`')]
        public void DefaultIdentifierDelimiterCanBeChanged(char newLeftDelimiter, char newRightDelimiter)
        {
            // when
            _options.SetSqlIdentifierDelimiters(newLeftDelimiter, newRightDelimiter);

            // then
            Assert.AreEqual(newLeftDelimiter, _options.SqlIdentifierLeftDelimiter);
            Assert.AreEqual(newRightDelimiter, _options.SqlIdentifierRightDelimiter);
        }

        [Test]
        public void CanTurnOffIdentifierDelimiting()
        {
            // when
            _options.UseDelimitedIdentifiers = false;

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
            _options = new MappingOptions
                {
                    ValidateSqlVersion = false
                };

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
            using (new Scope<MappingOptions>(new MappingOptions { BlankNodeTemplateSeparator = "x" }))
            {
                Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("x"));
            }

            // then
            Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("_"));
            Assert.That(MappingOptions.Default.BlankNodeTemplateSeparator, Is.EqualTo("_"));
        }

        [Test]
        public void Should_allow_changing_default_mapping_options()
        {
            // given
            MappingOptions.Default.BlankNodeTemplateSeparator = "y";

            // when
            using (new Scope<MappingOptions>(new MappingOptions { BlankNodeTemplateSeparator = "x" }))
            {
                Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("x"));
            }

            // then
            Assert.That(MappingOptions.Current.BlankNodeTemplateSeparator, Is.EqualTo("y"));
            Assert.That(MappingOptions.Default.BlankNodeTemplateSeparator, Is.EqualTo("y"));
        }
    }
}