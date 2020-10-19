using FluentValidation;
using FluentValidation.Results;

namespace SSRD.AdminUI.Template.Models
{
    public class AbstractValidatorWithNullCheck<T> : AbstractValidator<T>
    {
        protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Model can not be null"));
                return false;
            }

            return base.PreValidate(context, result);
        }
    }
}
