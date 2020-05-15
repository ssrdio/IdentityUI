using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class InviteRequest
    {
        public string Email { get; set; }
        public string RoleId { get; set; }

        public string GroupId { get; set; }
        public string GroupRoleId { get; set; }
    }

    internal class InviteRequestValidatior : AbstractValidator<InviteRequest>
    {
        public InviteRequestValidatior()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.RoleId);

            RuleFor(x => x.GroupId);

            RuleFor(x => x.GroupRoleId)
                .NotEmpty()
                .When(x => x.GroupId != null);
        }
    }
}
