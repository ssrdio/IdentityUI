using FluentValidation;
using SSRD.CommonUtils.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientConsentTableFilterModel
    {
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Scope { get; set; }

        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }

    public class ClientConsentTableFilterModelValidator : AbstractValidatorWithNullCheck<ClientConsentTableFilterModel>
    {
        public ClientConsentTableFilterModelValidator()
        {
            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From)
                .When(x => x.From != null);
        }
    }
}
