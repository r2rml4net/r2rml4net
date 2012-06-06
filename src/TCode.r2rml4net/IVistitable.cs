using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCode.r2rml4net
{
    public interface IVistitable<TVisitor>
    {
        void Accept<TVisitor>(TVisitor visitor);
    }
}
