using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models

{
    public class InviteToGroupRequest
    {
        public string Email { get; set; }
        public string GroupRoleId { get; set; }
    }

    internal class InviteToGroupRequestValidator : AbstractValidator<InviteToGroupRequest>
    {
        public InviteToGroupRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.GroupRoleId)
                .NotEmpty();
        }
    }
}
