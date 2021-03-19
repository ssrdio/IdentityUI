using FluentValidation;
using SSRD.CommonUtils.Validation;
using SSRD.IdentityUI.Core.Helper.Validator;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddWebAppClientModel
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public List<string> RedirectUrls { get; set; }
        public List<string> PostLogoutUrls { get; set; }
    }

    public class AddWebAppClientModelValidator : AbstractValidatorWithNullCheck<AddWebAppClientModel>
    {
        public AddWebAppClientModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ClientId)
                .NotEmpty();

            RuleFor(x => x.RedirectUrls.Where(c => !c.IsUrl()).Any())
                .Equal(false)
                .WithMessage("'{PropertyName}' is not a valid Url")
                .OverridePropertyName("RedirectUrls")
                .When(x => x.RedirectUrls != null);

            RuleFor(x => x.PostLogoutUrls.Where(c => !c.IsUrl()).Any())
                .Equal(false)
                .WithMessage("'{PropertyName}' is not a valid Url")
                .OverridePropertyName("PostLogoutUrls")
                .When(x => x.PostLogoutUrls != null);
        }
    }
}
