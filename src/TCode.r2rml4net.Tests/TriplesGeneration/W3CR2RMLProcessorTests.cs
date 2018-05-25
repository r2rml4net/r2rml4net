#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Moq;
using Xunit;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Log;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.RDF;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class W3CR2RMLProcessorTests
    {
        private W3CR2RMLProcessor _triplesGenerator;
        private readonly Mock<IR2RML> _r2RML;
        private readonly Mock<DbConnection> _connection;
        private readonly Mock<ITriplesMapProcessor> _triplesMapProcessor;
        private readonly Mock<IRdfHandler> _rdfHandler;
        private bool? _handlingResult;

        public W3CR2RMLProcessorTests()
        {
            _rdfHandler = new Mock<IRdfHandler>();
            _rdfHandler.Setup(handler => handler.EndRdf(It.IsAny<bool>()))
                       .Callback((bool result) => _handlingResult = result);
            _r2RML = new Mock<IR2RML>();
            _connection = new Mock<DbConnection>();
            _triplesMapProcessor = new Mock<ITriplesMapProcessor>();
            _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void ProcessesAllTriplesMaps(int triplesMapsCount)
        {
            // given
            var triplesMaps = GenerateTriplesMaps(triplesMapsCount).ToList();
            _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
            _triplesMapProcessor.Setup(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()));

            // when
            _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

            // then
            _r2RML.Verify(rml => rml.TriplesMaps, Times.Once());
            _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Exactly(triplesMapsCount));
            foreach (var triplesMap in triplesMaps)
            {
                ITriplesMap map = triplesMap;
                _triplesMapProcessor.Verify(rml => rml.ProcessTriplesMap(map, It.IsAny<DbConnection>(), It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
            }
            Assert.True(_handlingResult.HasValue && _handlingResult.Value);
            Assert.True(_triplesGenerator.Success);
        }

        IEnumerable<ITriplesMap> GenerateTriplesMaps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Mock<ITriplesMap>().Object;
            }
        }

        [Fact]
        public void CanStopProcessingIfDataErrorOccurs()
        {
            using (new MappingScope(new MappingOptions().IgnoringDataErrors(false)))
            {
                // given
                var triplesMaps = GenerateTriplesMaps(3).ToList();
                _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
                _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object);
                _triplesMapProcessor.Setup(
                    rml =>
                    rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(),
                                          It.IsAny<BlankNodeSubjectReplaceHandler>()))
                                    .Throws(new InvalidTermException(new Mock<ITermMap>().Object, "error"));

                // when
                _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

                // then
                _triplesMapProcessor.Verify(
                    rml =>
                    rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(),
                                          It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
                Assert.True(_handlingResult.HasValue && !_handlingResult.Value);
                Assert.False(_triplesGenerator.Success);
            }
        }

        [Fact]
        public void CanStopProcessingIfMappingErrorOccurs()
        {
            using (new MappingScope(new MappingOptions().IgnoringMappingErrors(false)))
            {
                // given
                var triplesMaps = GenerateTriplesMaps(3).ToList();
                _r2RML.Setup(rml => rml.TriplesMaps).Returns(triplesMaps);
                _triplesGenerator = new W3CR2RMLProcessor(_connection.Object, _triplesMapProcessor.Object);
                _triplesMapProcessor.Setup(
                    rml =>
                    rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(),
                                          It.IsAny<BlankNodeSubjectReplaceHandler>()))
                                    .Throws(new InvalidMapException("error"));

                // when
                _triplesGenerator.GenerateTriples(_r2RML.Object, _rdfHandler.Object);

                // then
                _triplesMapProcessor.Verify(
                    rml =>
                    rml.ProcessTriplesMap(It.IsAny<ITriplesMap>(), It.IsAny<DbConnection>(),
                                          It.IsAny<BlankNodeSubjectReplaceHandler>()), Times.Once());
                Assert.True(_handlingResult.HasValue && !_handlingResult.Value);
                Assert.False(_triplesGenerator.Success);
            }
        }

        [Fact]
        public void SettingLogFacadeShouldSetAllChildLoggers()
        {
            // given
            Mock<LogFacadeBase> logger = new Mock<LogFacadeBase>();

            // when
            _triplesGenerator.Log = logger.Object;

            // then
            _triplesMapProcessor.VerifySet(t => t.Log = logger.Object, Times.Once());
        }
    }
}
