using System;

namespace TCode.r2rml4net.Mapping
{
    public interface IUriValuedTermMap : ITermMap
    {
        /// <summary>
        /// Get the GraphUri URI or null if no URI has been set
        /// </summary>
        Uri URI { get; }
    }
}