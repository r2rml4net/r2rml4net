using System;

namespace TCode.r2rml4net.Exceptions
{
    public class InvalidSqlVersionException : InvalidMapException
    {
        public InvalidSqlVersionException(Uri uri)
            : base(string.Format("Sql version {0} is invalid", uri))
        {
        }
    }
}