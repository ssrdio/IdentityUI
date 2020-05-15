using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class InviteUserToGroupRequest
    {
        public string Email { get; set; }
    }

    internal class InviteUserToGroupRequestValidator : AbstractValidator<InviteUserToGroupRequest>
    {
        public InviteUserToGroupRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
