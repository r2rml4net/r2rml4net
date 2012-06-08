using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Interface for creating configuration of term maps as described on http://www.w3.org/TR/r2rml/#term-map
    /// </summary>
    public interface ITermMapConfiguration
    {
    }

    /// <summary>
    /// Interface for creating configuration of subject maps as described on http://www.w3.org/TR/r2rml/#subject-map
    /// </summary>
    public interface ISubjectMapConfiguration : ITermMapConfiguration
    {
        ISubjectMapConfiguration AddClass(Uri classIri);
        Uri[] ClassIris { get; }
    }
}
