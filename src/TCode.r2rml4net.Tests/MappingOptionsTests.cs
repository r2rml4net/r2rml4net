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
            Assert.AreEqual("_", _options.TemplateSeparator);
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
            _options.TemplateSeparator = newSeparator;

            // then
            Assert.AreEqual(newSeparator, _options.TemplateSeparator);
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
    }
}