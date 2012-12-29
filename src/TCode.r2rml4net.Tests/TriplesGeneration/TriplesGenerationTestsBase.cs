#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
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
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Moq;
using VDS.RDF;

namespace TCode.r2rml4net.Tests.TriplesGeneration
{
    public class TriplesGenerationTestsBase
    {
        protected IEnumerable<TMap> GenerateNMocks<TMap>(int count, params Tuple<Expression<Func<TMap, object>>, Func<object>>[] mockSetupFunctions) where TMap : class
        {
            for (int i = 0; i < count; i++)
            {
                var mock = new Mock<TMap>();
                foreach (var mockSetupFunction in mockSetupFunctions)
                {
                    mock.Setup(mockSetupFunction.Item1).Returns(mockSetupFunction.Item2);
                }
                yield return mock.Object;
            }
        }

        protected IUriNode CreateMockedUriNode(Uri uri)
        {
            var uriNode = new Mock<IUriNode>();
            uriNode.Setup(n => n.Uri).Returns(uri);
            return uriNode.Object;
        }

        protected static IDbCommand CreateCommandWithNRowsResult(int rowsCount, int fieldCount = 5)
        {
            int rowsReturned = 0;
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataReader> reader = new Mock<IDataReader>();
            reader.Setup(r => r.FieldCount).Returns(fieldCount);
            command.Setup(cmd => cmd.ExecuteReader()).Returns(reader.Object);
            for (int i = 0; i < fieldCount; i++)
            {
                int fieldIndex = i;
                reader.Setup(r => r.GetName(fieldIndex)).Returns(string.Format("Column{0}", i));
            }

            reader.Setup(rdr => rdr.Read()).Returns(() => rowsReturned++ < rowsCount);

            return command.Object;
        }
    }
}