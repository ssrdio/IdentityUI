using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Helper.Validator
{
    public class UrlValidator : PropertyValidator, IPredicateValidator
    {
        public UrlValidator() : base("'{PropertyName}' is not a valid Url")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return false;
            }

            bool isValid = Uri.IsWellFormedUriString((string)context.PropertyValue, UriKind.Absolute);

            return isValid;
        }
    }

    public static class UrlValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UrlValidator());
        }

        public static bool IsUrl(this string value)
        {
            if (value == null)
            {
                return false;
            }

            bool isValid = Uri.IsWellFormedUriString(value, UriKind.Absolute);

            return isValid;
        }
    }
}
