using System;
using System.Data;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Tests.RDF
{
    [TestFixture]
    public class DefaultSQLValuesMappingStrategyTests
    {
        private const int ColumnIndex = 1;

        private DefaultSQLValuesMappingStrategy _strategy;
        private Mock<IDataRecord> _logicalRow;

        [SetUp]
        public void Setup()
        {
            _strategy = new DefaultSQLValuesMappingStrategy();
            _logicalRow = new Mock<IDataRecord>();
        }

        [TestCase("System.Int64", XsdDatatypes.Integer)]
        [TestCase("System.Int32", XsdDatatypes.Integer)]
        [TestCase("System.Int16", XsdDatatypes.Integer)]
        [TestCase("System.DateTime", XsdDatatypes.DateTime)]
        [TestCase("System.Single", XsdDatatypes.Double)]
        [TestCase("System.Double", XsdDatatypes.Double)]
        [TestCase("System.Decimal", XsdDatatypes.Decimal)] 
        public void MapsDotnetTypes(string typeName, string uri)
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(Type.GetType(typeName));
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);

            // then
            Assert.IsNotNull(datatype);
            Assert.AreEqual(uri, datatype.ToString());
        }

        [Test]
        public void MapsDateToDate()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("DATE");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.IsNotNull(datatype);
            Assert.AreEqual(XsdDatatypes.Date, datatype.ToString());
        }

        [Test]
        public void MapsTimeToTime()
        {
            // given
            _logicalRow.Setup(row => row.GetFieldType(ColumnIndex)).Returns(typeof(DateTime));
            _logicalRow.Setup(row => row.GetDataTypeName(ColumnIndex)).Returns("TIME");
            _logicalRow.Setup(row => row.GetValue(ColumnIndex)).Returns(string.Empty);

            // when
            Uri datatype;
            _strategy.GetLexicalForm(ColumnIndex, _logicalRow.Object, out datatype);
            Assert.IsNotNull(datatype);
            Assert.AreEqual(XsdDatatypes.Time, datatype.ToString());
        }

        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void EnsuresBooleanIsLowercase(bool value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetValue(It.IsAny<int>())).Returns(value.ToString());

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Boolean));

            // then
            Assert.IsNotNull(valueString);
            Assert.AreEqual(expected, valueString);
            _logicalRow.VerifyAll();
        }

        [TestCase(-5.9, "-5.9E0")]
        [TestCase(+0.00014770215000, "1.4770215E-4")]
        [TestCase(01E+3, "1.0E3")]
        [TestCase(0, "0.0E0")]
        [TestCase(100.0, "1.0E2")]
        public void EnsuresProperyDoubleForm(double value, string expected)
        {
            // given
            _logicalRow.Setup(row => row.GetDouble(ColumnIndex)).Returns(value);

            // when
            var valueString = _strategy.GetMappedValue(ColumnIndex, _logicalRow.Object, new Uri(XsdDatatypes.Double));

            // then
            Assert.IsNotNull(valueString);
            Assert.AreEqual(expected, valueString);
            _logicalRow.VerifyAll();
        }
    }
}