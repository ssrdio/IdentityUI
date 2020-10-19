using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Attributes
{
    /// <summary>
    /// Targets using this attributes will not be logged if there are delete, because foreign key cascade delete
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class AuditIgnoreCascadeAttribute : Attribute
    {
    }
}
