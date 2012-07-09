using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class RefObjectMapConfigurationTests
    {
        RefObjectMapConfiguration _refObjectMap;

        [SetUp]
        public void Setup()
        {
            IGraph graph = new R2RMLConfiguration().R2RMLMappings;
            IUriNode referencedTriplesMap = graph.CreateUriNode(new Uri("http://test.example.com/TriplesMap"));
            IUriNode predicateObjectMap = graph.CreateUriNode(new Uri("http://test.example.com/PredicateObjectMap"));
            _refObjectMap = new RefObjectMapConfiguration(predicateObjectMap, referencedTriplesMap, graph);
        }

        [Test]
        public void CreatingAssertsRequiredTriples()
        {
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankObject("http://test.example.com/PredicateObjectMap", UriConstants.RrObjectMapProperty);
            _refObjectMap.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RrParentTriplesMapProperty, "http://test.example.com/TriplesMap");
        }
    }
}
