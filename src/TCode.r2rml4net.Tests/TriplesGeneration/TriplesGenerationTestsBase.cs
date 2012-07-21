using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;

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
    }
}