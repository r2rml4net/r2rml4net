#region Licence
// Copyright (C) 2013 Tomasz Pluskiewicz
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
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using NUnit.Framework;
using TCode.r2rml4net.Configuration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.Configuration
{
    public class ConfigurationTestsBase
    {
        protected static IGraph LoadConfiguration(string filename)
        {
            var configuration = new Graph();
            configuration.LoadFromEmbeddedResource(string.Format("TCode.r2rml4net.Tests.Configuration.Graphs.{0}, TCode.r2rml4net.Tests", filename));
            return configuration;
        }
    }

    [TestFixture]
    public class R2RMLObjectFactoryTests : ConfigurationTestsBase
    {
        private R2RMLObjectFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new R2RMLObjectFactory();
        }

        [TestCase(typeof(W3CR2RMLProcessor))]
        [TestCase(typeof(DirectR2RMLMapping))]
        public void Factory_should_allow_loading_R2RML4NET_types(Type typeToLoad)
        {
            Assert.That(_factory.CanLoadObject(typeToLoad), Is.True);
        }

        [Test]
        public void Factory_should_succesfully_load_processor_with_complete_custom_options()
        {
            // given
            var configuration = LoadConfiguration("w3cprocessor.ttl");

            // when
            object processor;
            var loadResult = _factory.TryLoadObject(configuration, configuration.GetUriNode("ex:fullyLoadedProcessorForMsSql"), typeof(W3CR2RMLProcessor), out processor);

            // then
            var procesorTyped = processor as W3CR2RMLProcessor;
            Assert.That(loadResult, Is.True);
            Assert.That(procesorTyped, Is.Not.Null);
            Assert.That(procesorTyped.Options.BlankNodeTemplateSeparator, Is.EqualTo("x"));
            Assert.That(procesorTyped.Options.UseDelimitedIdentifiers, Is.EqualTo(false));
            Assert.That(procesorTyped.Options.SqlIdentifierRightDelimiter, Is.EqualTo('y'));
            Assert.That(procesorTyped.Options.SqlIdentifierLeftDelimiter, Is.EqualTo('y'));
            Assert.That(procesorTyped.Options.ValidateSqlVersion, Is.EqualTo(true));
            Assert.That(procesorTyped.Options.IgnoreDataErrors, Is.EqualTo(false));
            Assert.That(procesorTyped.Options.IgnoreMappingErrors, Is.EqualTo(false));
            Assert.That(procesorTyped.Options.PreserveDuplicateRows, Is.EqualTo(false));
        }

        [TestCase("ex:processorForOdbc", typeof(OdbcConnection))]
        [TestCase("ex:processorForOleDb", typeof(OleDbConnection))]
        [TestCase("ex:fullyLoadedProcessorForMsSql", typeof(SqlConnection))]
        public void Factory_should_succesfully_load_processor_for_connection_type(string qname, Type connectionType)
        {
            // given
            var configuration = LoadConfiguration("w3cprocessor.ttl");

            // when
            object processor;
            var loadResult = _factory.TryLoadObject(configuration, configuration.GetUriNode(qname), typeof(W3CR2RMLProcessor), out processor);

            // then
            var procesorTyped = processor as W3CR2RMLProcessor;
            Assert.That(loadResult, Is.True);
            Assert.That(procesorTyped, Is.Not.Null);
            Assert.That(procesorTyped.Connection, Is.InstanceOf(connectionType));
        }
    }
}