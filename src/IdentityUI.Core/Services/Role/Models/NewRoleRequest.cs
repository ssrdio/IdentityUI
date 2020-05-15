using FluentValidation;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role.Models
{
    public class NewRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleTypes? Type { get; set; }
    }

    internal class NewRoleValidator : AbstractValidator<NewRoleRequest>
    {
        public NewRoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(256);

            RuleFor(x => x.Type)
                .NotNull()
                .IsInEnum();
        }
    }
}
