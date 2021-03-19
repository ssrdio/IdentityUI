using FluentValidation;
using SSRD.CommonUtils.Validation;
using SSRD.IdentityUI.Core.Helper.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class UpdateClientModel
    {
        public string Name { get; set; }

        public List<string> RedirectUrls { get; set; }
        public List<string> PostLogoutUrls { get; set; }

        public List<string> Endpoints { get; set; }
        public List<string> GrantTypes { get; set; }
        public List<string> ResponseTypes { get; set; }

        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }
    }

    public class UpdateClientModelValidator : AbstractValidatorWithNullCheck<UpdateClientModel>
    {
        public UpdateClientModelValidator()
        {
            RuleFor(x => x.Name)
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
                .When(x => x.RedirectUrls != null);
        }
    }
}
