using FluentValidation;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Services.User.Models;

namespace IdentityUI.Sample.UserAttributes
{
    public class UserAttributeValidator : AbstractValidatorWithNullCheck<RegisterRequest>
    {
        public UserAttributeValidator()
        {
            RuleFor(x => x.Attributes["addressLine1"])
                .NotEmpty()
                .OverridePropertyName("Attributes[addressLine1]")
                .WithMessage("'Address Line 1', must not be empty");

            RuleFor(x => x.Attributes["city"])
                .NotEmpty()
                .OverridePropertyName("Attributes[city]")
                .WithMessage("'City', must not be empty");

            RuleFor(x => x.Attributes["zipCode"])
                .NotEmpty()
                .OverridePropertyName("Attributes[zipCode]")
                .WithMessage("'Zip/Postal Code', must not be empty");

            RuleFor(x => x.Attributes["country"])
                .NotEmpty()
                .OverridePropertyName("Attributes[country]")
                .WithMessage("'Country', must not be empty");
        }
    }
}
