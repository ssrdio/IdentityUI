using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using SSRD.IdentityUI.Core.Helper.Validator;
using System.Linq;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddSinglePageClientModel
    {
        public string Name { get; set; }
        public string ClientId { get; set; }

        public List<string> RedirectUrls { get; set; }
        public List<string> PostLogoutUrls { get; set; }
    }

    public class AddSinglePageClientModelValidator : AbstractValidatorWithNullCheck<AddSinglePageClientModel>
    {
        public AddSinglePageClientModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ClientId)
                .NotEmpty();

            RuleFor(x => x.RedirectUrls)
                .NotEmpty();

            RuleFor(x => x.RedirectUrls.Where(c => !c.IsUrl()).Any())
                .Equal(false)
                .WithMessage("'{PropertyName}' is not a valid Url")
                .OverridePropertyName("RedirectUrls")
                .When(x => x.RedirectUrls != null);

            RuleFor(x => x.PostLogoutUrls)
                .NotEmpty();

            RuleFor(x => x.PostLogoutUrls.Where(c => !c.IsUrl()).Any())
                .Equal(false)
                .WithMessage("'{PropertyName}' is not a valid Url")
                .OverridePropertyName("PostLogoutUrls")
                .When(x => x.PostLogoutUrls != null);
        }
    }
}
