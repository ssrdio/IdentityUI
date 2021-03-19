using System;

namespace SSRD.CommonUtils.Validation
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ValidationRuleSetAttribute : Attribute
    {
        public readonly string[] RuleSets;

        public ValidationRuleSetAttribute(params string[] ruleSets)
        {
            RuleSets = ruleSets;
        }
    }
}
