using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role.Models
{
    public class AddRolePermissionRequest
    {
        public string PermissionId { get; set; }
    }

    internal class AddRolePermissionRequestValidator : AbstractValidator<AddRolePermissionRequest>
    {
        public AddRolePermissionRequestValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty();
        }
    }
}
