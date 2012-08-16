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