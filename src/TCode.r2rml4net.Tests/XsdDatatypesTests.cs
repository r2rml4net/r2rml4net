using System;
using NUnit.Framework;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Tests
{
    [TestFixture]
    public class XsdDatatypesTests
    {
        [TestCase(R2RMLType.Binary, "http://www.w3.org/2001/XMLSchema#hexBinary")]
        [TestCase(R2RMLType.Boolean, "http://www.w3.org/2001/XMLSchema#boolean")]
        [TestCase(R2RMLType.Date, "http://www.w3.org/2001/XMLSchema#date")]
        [TestCase(R2RMLType.DateTime, "http://www.w3.org/2001/XMLSchema#dateTime")]
        [TestCase(R2RMLType.Decimal, "http://www.w3.org/2001/XMLSchema#decimal")]
        [TestCase(R2RMLType.FloatingPoint, "http://www.w3.org/2001/XMLSchema#double")]
        [TestCase(R2RMLType.Integer, "http://www.w3.org/2001/XMLSchema#integer")]
        [TestCase(R2RMLType.Time, "http://www.w3.org/2001/XMLSchema#time")]
        public void ReturnsCorrectUrisForNonStringDatatypes(R2RMLType type, string expectedUri)
        {
            // when
            Uri uri = XsdDatatypes.GetDataType(type);

            // then
            Assert.IsNotNull(uri);
            Assert.AreEqual(expectedUri, uri.ToString());
        }

        [TestCase(R2RMLType.String)]
        [TestCase(R2RMLType.Undefined)]
        public void ReturnsNullForStringOrUndefinedDatatypes(R2RMLType type)
        {
            Assert.IsNull(XsdDatatypes.GetDataType(type));
        }
    }
}
