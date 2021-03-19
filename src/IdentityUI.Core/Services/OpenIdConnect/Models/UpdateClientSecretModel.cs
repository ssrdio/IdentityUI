using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class UpdateClientSecretModel
    {
        public string ClientSecret { get; set; }
    }

    public class UpdateClientSecretModelValidator : AbstractValidatorWithNullCheck<UpdateClientSecretModel>
    {
        public UpdateClientSecretModelValidator()
        {
            RuleFor(x => x.ClientSecret)
                .NotEmpty();
        }
    }
}
