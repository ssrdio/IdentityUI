using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Attributes
{
    /// <summary>
    /// Targets with this attribute will not get logged in audit log
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = true)]
    public sealed class AuditIgnoreAttribute : Attribute
    {
    }
}
