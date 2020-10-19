using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public sealed class AuditIgnoreViewData : Attribute
    {
    }
}
