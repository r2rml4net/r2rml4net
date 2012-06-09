using System;

namespace TCode.r2rml4net.Mapping
{
    /// <summary>
    /// Represents a validation error in Triples Map structure
    /// </summary>
    public class InvalidTriplesMapException : Exception
    {
        /// <summary>
        /// Uri of node where error occured
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="InvalidTriplesMapException"/> with a message
        /// </summary>
        public InvalidTriplesMapException(string message, Uri errorneousNodeUri)
            : base(message)
        {
            Uri = errorneousNodeUri;
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidTriplesMapException"/> with a message
        /// </summary>
        public InvalidTriplesMapException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Returns a message passed to the constructor and Uri of node where error occured
        /// </summary>
        public override string Message
        {
            get
            {
                return Uri != null 
                    ? string.Format("{0}. Error in node {1}", base.Message, Uri) 
                    : base.Message;
            }
        }
    }
}
