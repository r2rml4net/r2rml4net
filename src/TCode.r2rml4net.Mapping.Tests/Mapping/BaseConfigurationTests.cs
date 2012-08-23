using System;
using Moq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Tests.Mocks;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class BaseConfigurationTests
    {
        [Test]
        public void MappingOptionsCannotBeNull()
        {
            ArgumentNullException exception;

            var uri = new Uri("http://some.uri");
            exception = Assert.Throws<ArgumentNullException>(() => new MockBaseConfiguration(uri, null));
            Assert.AreEqual("mappingOptions", exception.ParamName);

            var graph = new Mock<IGraph>().Object;
            var node = new Mock<INode>().Object;
            exception = Assert.Throws<ArgumentNullException>(() => new MockBaseConfiguration(graph, node, null));
            Assert.AreEqual("mappingOptions", exception.ParamName);

            exception = Assert.Throws<ArgumentNullException>(() => new MockBaseConfiguration(graph, null));
            Assert.AreEqual("mappingOptions", exception.ParamName);

            ITriplesMapConfiguration triplesMap = new Mock<ITriplesMapConfiguration>().Object;
            exception = Assert.Throws<ArgumentNullException>(() => new MockBaseConfiguration(triplesMap, graph, node, null));
            Assert.AreEqual("mappingOptions", exception.ParamName);
        }
    }
}