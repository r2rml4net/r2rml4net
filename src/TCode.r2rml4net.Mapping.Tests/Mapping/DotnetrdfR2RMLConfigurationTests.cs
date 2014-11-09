#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.Mapping.Fluent;

namespace TCode.r2rml4net.Mapping.Tests.Mapping
{
    [TestFixture]
    public class DotnetrdfR2RMLConfigurationTests
    {
        private FluentR2RML _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new FluentR2RML();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(20)]
        public void CanCreateTriplesMapsFromConcreteTable(int numberOfTables)
        {
            IList<ITriplesMapConfiguration> tripleMaps = new List<ITriplesMapConfiguration>(numberOfTables);

            for (int i = 0; i < numberOfTables; i++)
            {
                tripleMaps.Add(_configuration.CreateTriplesMapFromTable("TableName"));
            }

            Assert.IsTrue(tripleMaps.All(map => map != null));
            foreach (var configuration in tripleMaps)
            {
                Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(20)]
        public void CanCreateTriplesMapsFromR2RMLView(int numberOfTables)
        {
            IList<ITriplesMapConfiguration> tripleMaps = new List<ITriplesMapConfiguration>(numberOfTables);

            for (int i = 0; i < numberOfTables; i++)
            {
                tripleMaps.Add(_configuration.CreateTriplesMapFromR2RMLView("SELECT * from Table"));
            }

            Assert.IsTrue(tripleMaps.All(map => map != null));
            foreach (var configuration in tripleMaps)
            {
                Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            }
        }

        [Test]
        public void SqlVersionUriCanBeChanged()
        {
            ITriplesMapConfiguration configuration = _configuration.CreateTriplesMapFromR2RMLView("SELECT...")
                                                                   .SetSqlVersion(new Uri("http://www.w3.org/ns/r2rml#SQL2008"));

            Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }

        [Test]
        public void SqlVersionUriCanBeChangedFromUriString()
        {
            ITriplesMapConfiguration configuration = _configuration.CreateTriplesMapFromR2RMLView("SELECT...")
                                                                   .SetSqlVersion("http://www.w3.org/ns/r2rml#SQL2008");

            Assert.IsInstanceOf<TriplesMapConfiguration>(configuration);
            Assert.IsNotNull(configuration);
        }

        [Test]
        public void CreatingTriplesMapFromTableNameAssertsTriplesInGraph()
        {
            // given
            const string tablename = "TableName";
            string triplesMapUri = string.Format("{0}{1}TriplesMap", _configuration.R2RMLMappings.BaseUri, tablename);

            // when
            var triplesMap = _configuration.CreateTriplesMapFromTable(tablename);

            // then
            Assert.AreEqual(triplesMapUri, triplesMap.Uri.AbsoluteUri);
            _configuration.R2RMLMappings.VerifyHasTriple(triplesMapUri, UriConstants.RdfType, UriConstants.RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankObject(triplesMapUri, UriConstants.RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(UriConstants.RrTableNameProperty, tablename);
        }

        [Test]
        public void CreatingTriplesMapFromR2RMLViewAssertsTriplesInGraph()
        {
            // given
            const string sqlQuery = "SELECT * from X";

            // when
            _configuration.CreateTriplesMapFromR2RMLView(sqlQuery);

            // then
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubject(UriConstants.RdfType, UriConstants.RrTriplesMapClass);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndObject(UriConstants.RrLogicalTableProperty);
            _configuration.R2RMLMappings.VerifyHasTripleWithBlankSubjectAndLiteralObject(UriConstants.RrSqlQueryProperty, sqlQuery);
        }

        [Test]
        public void ConfigurationBuilderCreatedWithAnEmptyGraph()
        {
            Assert.IsTrue(_configuration.R2RMLMappings.IsEmpty);
        }

        [Test]
        public void ConfigurationBuilderCreatedWithGraphWithDefaultNamespaces()
        {
            Assert.IsTrue(_configuration.R2RMLMappings.NamespaceMap.HasNamespace("rr"));
            Assert.AreEqual("http://www.w3.org/ns/r2rml#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rr").AbsoluteUri);

            Assert.IsTrue(_configuration.R2RMLMappings.NamespaceMap.HasNamespace("rdf"));
            Assert.AreEqual("http://www.w3.org/1999/02/22-rdf-syntax-ns#", _configuration.R2RMLMappings.NamespaceMap.GetNamespaceUri("rdf").AbsoluteUri);
        }

        [Test]
        public void ConfigurationBuilderConstructedWithDefaultBaseUri()
        {
            Assert.AreEqual(FluentR2RML.DefaultBaseUri, _configuration.R2RMLMappings.BaseUri);
        }

        [Test]
        public void ConfigurationBuilderCanBeConstructedWithChangedDefaultBaseUri()
        {
            Uri BaseUri = new Uri("http://this.is.test.com/rdf/");

            _configuration = new FluentR2RML(BaseUri);

            Assert.AreEqual(BaseUri, _configuration.R2RMLMappings.BaseUri);
        }
    }
}
