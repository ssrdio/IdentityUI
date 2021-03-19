using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientTokenTableFilterModel
    {
        public string Status { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }

        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }

    public class ClientTokenTableFilterModelValidator : AbstractValidator<ClientTokenTableFilterModel>
    {
        public ClientTokenTableFilterModelValidator()
        {
            RuleFor(x => x.Status);

            RuleFor(x => x.Type);

            RuleFor(x => x.Subject);

            RuleFor(x => x.From)
                .LessThanOrEqualTo(x => x.To)
                .When(x => x.To != null);

            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From)
                .When(x => x.From != null);
        }
    }
}
