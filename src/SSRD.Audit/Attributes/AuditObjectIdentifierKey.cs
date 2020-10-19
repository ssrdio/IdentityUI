using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AuditObjectIdentifierKey : Attribute
    {
        public string ObjectKey { get; }

        public AuditObjectIdentifierKey(string objectKey)
        {
            ObjectKey = objectKey;
        }
    }
}
