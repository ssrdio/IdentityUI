using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddCustomClientModel
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
    }

    public class AddCustomClientModelValidator : AbstractValidatorWithNullCheck<AddCustomClientModel>
    {
        public AddCustomClientModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ClientId)
                .NotEmpty();
        }
    }
}
