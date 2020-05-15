using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role.Models
{
    public class AddRoleAssignmentRequest
    {
        public string RoleId { get; set; }
    }

    internal class AddRoleAssignmentRequestValidator : AbstractValidator<AddRoleAssignmentRequest>
    {
        public AddRoleAssignmentRequestValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty();
        }
    }
}
