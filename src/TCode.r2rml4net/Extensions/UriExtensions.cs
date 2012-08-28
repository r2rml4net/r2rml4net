using System;
using System.Reflection;

namespace TCode.r2rml4net.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Uri"/> class
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// A pretty dirty workaround to have slashes unescaped to avoid relative
        /// <see cref="Uri"/> resolution and simmilar malfunctions
        /// </summary>
        /// <remarks>See http://stackoverflow.com/questions/2320533/system-net-uri-with-urlencoded-characters</remarks>
        public static void LeaveDotsAndSlashesEscaped(this Uri uri)
        {
            const int unEscapeDotsAndSlashes = 0x2000000;
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            FieldInfo fieldInfo = uri.GetType().GetField("m_Syntax", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new MissingFieldException("'m_Syntax' field not found");
            }
            object uriParser = fieldInfo.GetValue(uri);

            fieldInfo = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new MissingFieldException("'m_Flags' field not found");
            }
            object uriSyntaxFlags = fieldInfo.GetValue(uriParser);

            // Clear the flag that we don't want
            uriSyntaxFlags = (int)uriSyntaxFlags & ~unEscapeDotsAndSlashes;

            fieldInfo.SetValue(uriParser, uriSyntaxFlags);
        }
    }
}