using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role.Models
{
    public class EditRoleRequest
    {
        public string Description { get; set; }
    }

    internal class EditRoleValidator : AbstractValidator<EditRoleRequest>
    {
        public EditRoleValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(256);
        }
    }
}
