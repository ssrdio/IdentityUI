using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services
{
    public class InviteRequest
    {
        public string Email { get; set; }
        public string GroupId { get; set; }
    }

    internal class InviteRequestValidator : AbstractValidator<InviteRequest>
    {
        public InviteRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.GroupId);
        }
    }
}
