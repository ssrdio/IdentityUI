using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.CommonUtils.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class SkipModelValidationAttribute : Attribute
    {
    }
}
