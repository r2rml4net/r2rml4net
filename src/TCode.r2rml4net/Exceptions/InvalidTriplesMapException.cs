using System;
using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Exceptions
{
    /// <summary>
    /// Represents a validation error in Triples Map structure
    /// </summary>
    public class InvalidTriplesMapException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="InvalidTriplesMapException"/> with a message
        /// </summary>
        public InvalidTriplesMapException(string message, IMapBase triplesMap)
            : base(string.Format("Map {0} is invalid. {1}", triplesMap.Node, message))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidTriplesMapException"/> with a message
        /// </summary>
        public InvalidTriplesMapException(string message)
            : base(message)
        {
        }
    }
}
