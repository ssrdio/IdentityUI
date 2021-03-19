using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddClientSecretModel
    {
        public string ClientSecret { get; set; }
    }

    public class AddClientSecretModelValidator : AbstractValidatorWithNullCheck<AddClientSecretModel>
    {
        public AddClientSecretModelValidator()
        {
            RuleFor(x => x.ClientSecret)
                .NotEmpty();
        }
    }
}
