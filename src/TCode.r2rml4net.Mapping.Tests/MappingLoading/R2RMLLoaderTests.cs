using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using TCode.r2rml4net.RDF;

namespace TCode.r2rml4net.Mapping.Tests.MappingLoading
{
    [TestFixture]
    public class R2RMLLoaderTests
    {
        #region Simple Test Graph
        private const string TestGraph =
            @"@prefix rr: <http://www.w3.org/ns/r2rml#> .
                                        @prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
                                        @base <http://mappingpedia.org/rdb2rdf/r2rml/tc/> .

                                        <StudentTriplesMap>
                                             a rr:TriplesMap;
    
                                            rr:logicalTable [ rr:tableName ""Student""; ];

                                            rr:subjectMap 
	                                        [ 
	                                          rr:termType rr:BlankNode;  
	                                          rr:class <http://example.com/Student>
	                                        ];

                                            rr:predicateObjectMap
                                            [ 
                                              rr:predicateMap	[ rr:constant <http://example.com/Student#Name> ] ;
                                              rr:objectMap		[ rr:column ""Name"" ]
                                            ]
                                            ."; 
        #endregion

        [Test]
        public void CanLoadR2RMLFromString()
        {
            // when
            IR2RML mappings = R2RMLLoader.Load(TestGraph);

            // then
            Assert.IsNotNull(mappings);
        }

        [Test]
        public void CanLoadR2RMLFromStream()
        {
            // given
            using (Stream graphStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(graphStream))
                {
                    writer.Write(TestGraph);
                    graphStream.Seek(0, SeekOrigin.Begin);
                }

                // when
                IR2RML mappings = R2RMLLoader.Load(graphStream);

                // then
                Assert.IsNotNull(mappings);
            }
        }

        [Test]
        public void CanLoadComplexMapping()
        {
            IR2RML mappings;

            using (Stream turtle = Assembly.GetExecutingAssembly().GetManifestResourceStream("TCode.r2rml4net.Mapping.Tests.MappingLoading.ComplexTestGraph.ttl, TCode.r2rml4net.Mapping.Tests"))
            {
                mappings = R2RMLLoader.Load(turtle);
            }

            Assert.AreEqual(5, mappings.TriplesMaps.Count());
        }
    }
}
