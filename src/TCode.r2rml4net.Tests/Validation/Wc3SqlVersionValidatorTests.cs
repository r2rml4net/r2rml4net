using System;
using NUnit.Framework;
using TCode.r2rml4net.Validation;

namespace TCode.r2rml4net.Tests.Validation
{
    [TestFixture]
    public class Wc3SqlVersionValidatorTests
    {
        private Wc3SqlVersionValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new Wc3SqlVersionValidator();
        }

        [TestCase("http://www.w3.org/ns/r2rml#SQL2008")]
        [TestCase("http://www.w3.org/ns/r2rml#Oracle")]
        [TestCase("http://www.w3.org/ns/r2rml#MySQL")]
        [TestCase("http://www.w3.org/ns/r2rml#MSSQLServer")]
        [TestCase("http://www.w3.org/ns/r2rml#HSQLDB")]
        [TestCase("http://www.w3.org/ns/r2rml#PostgreSQL")]
        [TestCase("http://www.w3.org/ns/r2rml#DB2")]
        [TestCase("http://www.w3.org/ns/r2rml#Informix")]
        [TestCase("http://www.w3.org/ns/r2rml#Ingres")]
        [TestCase("http://www.w3.org/ns/r2rml#Progress")]
        [TestCase("http://www.w3.org/ns/r2rml#SybaseASE")]
        [TestCase("http://www.w3.org/ns/r2rml#SybaseSQLAnywhere")]
        [TestCase("http://www.w3.org/ns/r2rml#Virtuoso")]
        [TestCase("http://www.w3.org/ns/r2rml#Firebird")]
        public void ReturnsTrueForValidLanguageTags(string tagString)
        {
            Uri tag = new Uri(tagString);
            Assert.IsTrue(_validator.SqlVersionIsValid(tag));
        }

        [Test]
        public void ReturnsFalseForInvalidLanguageTags()
        {
            Uri tag = new Uri("http://www.w3.org/ns/r2rml#Firebird");
            Assert.IsTrue(_validator.SqlVersionIsValid(tag));
        }
    }
}